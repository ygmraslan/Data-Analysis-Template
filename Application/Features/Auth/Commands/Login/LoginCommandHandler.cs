using DataAnalysis.Application.Common;
using DataAnalysis.Application.Common.Interfaces;
using DataAnalysis.Application.Common.Settings;
using DataAnalysis.Application.Features.Auth.Abstractions;
using DataAnalysis.Application.Features.AuthLogs.Abstractions;
using DataAnalysis.Domain.Entities.Identity;
using DataAnalysis.Domain.Entities.Logging;
using DataAnalysis.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace DataAnalysis.Application.Features.Auth.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<LoginCommandResponse>>
{
    private readonly IAuthRepository _authRepository;
    private readonly IAuthLogRepository _authLogRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordService _passwordService;
    private readonly ITokenService _tokenService;
    private readonly JwtSettings _jwtSettings;
    private readonly ILogger<LoginCommandHandler> _logger;

    public LoginCommandHandler(
        IAuthRepository authRepository,
        IAuthLogRepository authLogRepository,
        IUnitOfWork unitOfWork,
        IPasswordService passwordService,
        ITokenService tokenService,
        IOptions<JwtSettings> jwtSettings,
        ILogger<LoginCommandHandler> logger)
    {
        _authRepository = authRepository;
        _authLogRepository = authLogRepository;
        _unitOfWork = unitOfWork;
        _passwordService = passwordService;
        _tokenService = tokenService;
        _jwtSettings = jwtSettings.Value;
        _logger = logger;
    }

    public async Task<Result<LoginCommandResponse>> Handle(
        LoginCommand request,
        CancellationToken cancellationToken)
    {
        var user = await _authRepository.FindByEmailAsync(request.Email, cancellationToken);

        if (user == null)
        {
            _logger.LogWarning("Login failed. User not found: {Email}", request.Email);
            await WriteAuthLogAsync(null, request, success: false, reason: "Kullanıcı bulunamadı");
            return Result<LoginCommandResponse>.Fail("Invalid email or password.", ErrorCodes.Auth.InvalidCredentials);
        }

        if (user.LockoutEnd.HasValue && user.LockoutEnd > DateTime.UtcNow)
        {
            _logger.LogWarning("Login failed. Account locked. UserId: {UserId}", user.Id);
            await WriteAuthLogAsync(user, request, success: false, reason: $"Hesap kilitli. Kilit bitiş: {user.LockoutEnd.Value.ToLocalTime():HH:mm}");
            return Result<LoginCommandResponse>.Fail(
                $"Account is locked. Please try again after {user.LockoutEnd.Value.ToLocalTime():HH:mm}.",
                ErrorCodes.Auth.AccountLocked);
        }

        if (!_passwordService.VerifyPassword(request.Password, user.PasswordHash))
        {
            user.FailedLoginAttempts++;

            if (user.FailedLoginAttempts >= 3)
            {
                user.LockoutEnd = DateTime.UtcNow.AddMinutes(15);
                user.FailedLoginAttempts = 0;
                _logger.LogWarning("Account locked due to too many failed attempts. UserId: {UserId}", user.Id);
                await WriteAuthLogAsync(user, request, success: false, reason: "Çok fazla hatalı giriş — hesap 15 dakika kilitlendi");
            }
            else
            {
                _logger.LogWarning("Login failed. Invalid password. UserId: {UserId}, Attempt: {Attempt}", user.Id, user.FailedLoginAttempts);
                await WriteAuthLogAsync(user, request, success: false, reason: $"Hatalı şifre (deneme: {user.FailedLoginAttempts})");
            }

            await _unitOfWork.SaveChangesAsync();
            return Result<LoginCommandResponse>.Fail("Invalid email or password.", ErrorCodes.Auth.InvalidCredentials);
        }

        if (user.FailedLoginAttempts > 0 || user.LockoutEnd.HasValue)
            await _authRepository.ResetFailedLoginAttemptsAsync(user.Id, cancellationToken);

        if (user.IsPasswordChangeRequired)
        {
            await _authRepository.UpdateLastLoginDateAsync(user.Id, cancellationToken);
            _logger.LogInformation("Login successful. Password change required. UserId: {UserId}", user.Id);
            await WriteAuthLogAsync(user, request, success: true, reason: "Giriş başarılı — şifre değişikliği gerekiyor");

            var passwordChangeToken = _tokenService.GeneratePasswordChangeToken(user.Id);
            return Result<LoginCommandResponse>.Ok(new LoginCommandResponse
            {
                RequiresPasswordChange = true,
                PasswordChangeToken = passwordChangeToken
            });
        }

        var pendingMfa = await _authRepository.FindPendingMfaByUserIdAsync(user.Id, cancellationToken);

        if (pendingMfa == null)
            pendingMfa = await _authRepository.FindResetMfaByUserIdAsync(user.Id, cancellationToken);

        if (pendingMfa != null)
        {
            await _authRepository.UpdateLastLoginDateAsync(user.Id, cancellationToken);
            _logger.LogInformation("Login successful. MFA setup required. UserId: {UserId}", user.Id);
            await WriteAuthLogAsync(user, request, success: true, reason: "Giriş başarılı — MFA kurulumu gerekiyor");

            var mfaToken = await IssueMfaSessionTokenAsync(user.Id, request.IpAddress, cancellationToken);
            return Result<LoginCommandResponse>.Ok(new LoginCommandResponse
            {
                RequiresMfaSetup = true,
                MfaToken = mfaToken
            });
        }

        var activeMfa = await _authRepository.FindActiveMfaByUserIdAsync(user.Id, cancellationToken);

        if (activeMfa != null)
        {
            await _authRepository.UpdateLastLoginDateAsync(user.Id, cancellationToken);
            _logger.LogInformation("Login successful. MFA required. UserId: {UserId}", user.Id);
            await WriteAuthLogAsync(user, request, success: true, reason: "Giriş başarılı — MFA doğrulaması bekleniyor");

            var mfaToken = await IssueMfaSessionTokenAsync(user.Id, request.IpAddress, cancellationToken);
            return Result<LoginCommandResponse>.Ok(new LoginCommandResponse
            {
                RequiresMfa = true,
                MfaToken = mfaToken
            });
        }

        var accessToken = _tokenService.GenerateAccessToken(user.Id, user.Email);
        var refreshToken = _tokenService.GenerateRefreshToken();
        var refreshTokenHash = _tokenService.HashToken(refreshToken);

        try
        {
            await _unitOfWork.BeginTransactionAsync();

            await _authRepository.AddRefreshTokenAsync(new RefreshToken
            {
                UserId = user.Id,
                Token = refreshTokenHash,
                ExpiresDate = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpireDays),
                IpAddress = request.IpAddress
            }, cancellationToken);

            await _authRepository.UpdateLastLoginDateAsync(user.Id, cancellationToken);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Login transaction failed. UserId: {UserId}", user.Id);
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }

        _logger.LogInformation("Login successful. UserId: {UserId}, IpAddress: {IpAddress}", user.Id, request.IpAddress);
        await WriteAuthLogAsync(user, request, success: true, reason: "Giriş başarılı");

        return Result<LoginCommandResponse>.Ok(new LoginCommandResponse
        {
            RequiresMfa = false,
            AccessToken = accessToken,
            RefreshToken = refreshToken
        });
    }

    private async Task<string> IssueMfaSessionTokenAsync(int userId, string ipAddress, CancellationToken ct)
    {
        var mfaToken = _tokenService.GenerateMfaToken();
        var mfaTokenHash = _tokenService.HashToken(mfaToken);

        await _authRepository.AddMfaSessionTokenAsync(new MfaSessionToken
        {
            UserId = userId,
            Token = mfaTokenHash,
            ExpiresDate = DateTime.UtcNow.AddMinutes(_jwtSettings.MfaTokenExpireMinutes),
            IpAddress = ipAddress
        }, ct);

        await _unitOfWork.SaveChangesAsync();

        return mfaToken;
    }

    private async Task WriteAuthLogAsync(User? user, LoginCommand request, bool success, string reason)
    {
        try
        {
            var log = new AuthLog
            {
                UserId    = user?.Id,
                Email     = user?.Email ?? request.Email,
                Success   = success,
                Reason    = reason,
                IpAddress = request.IpAddress,
                UserAgent = request.UserAgent,
                Browser   = BrowserParser.Parse(request.UserAgent),
            };

            await _authLogRepository.AddAsync(log);
            await _unitOfWork.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "AuthLog write failed. Email: {Email}", request.Email);
        }
    }
}
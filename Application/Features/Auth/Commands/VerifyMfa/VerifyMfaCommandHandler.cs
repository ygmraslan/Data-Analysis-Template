using DataAnalysis.Application.Common;
using DataAnalysis.Application.Common.Interfaces;
using DataAnalysis.Application.Common.Settings;
using DataAnalysis.Application.Features.Auth.Abstractions;
using DataAnalysis.Domain.Entities.Identity;
using DataAnalysis.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DataAnalysis.Application.Features.Auth.Commands.VerifyMfa;

public class VerifyMfaCommandHandler : IRequestHandler<VerifyMfaCommand, Result<VerifyMfaCommandResponse>>
{
    private readonly IAuthRepository _authRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenService _tokenService;
    private readonly IMfaService _mfaService;
    private readonly JwtSettings _jwtSettings;
    private readonly ILogger<VerifyMfaCommandHandler> _logger;

    public VerifyMfaCommandHandler(
        IAuthRepository authRepository,
        IUnitOfWork unitOfWork,
        ITokenService tokenService,
        IMfaService mfaService,
        IOptions<JwtSettings> jwtSettings,
        ILogger<VerifyMfaCommandHandler> logger)
    {
        _authRepository = authRepository;
        _unitOfWork = unitOfWork;
        _tokenService = tokenService;
        _mfaService = mfaService;
        _jwtSettings = jwtSettings.Value;
        _logger = logger;
    }

  public async Task<Result<VerifyMfaCommandResponse>> Handle(
    VerifyMfaCommand request,
    CancellationToken cancellationToken)
{
    var tokenHash = _tokenService.HashToken(request.MfaToken);
    var session = await _authRepository.FindActiveMfaSessionTokenAsync(tokenHash, cancellationToken);

    if (session == null)
    {
        _logger.LogWarning("MFA verification failed: Invalid or expired MFA token.");
        return Result<VerifyMfaCommandResponse>.Fail("Invalid or expired MFA token.", ErrorCodes.Auth.InvalidToken);
    }

    var user = await _authRepository.FindByIdAsync(session.UserId, cancellationToken);
    if (user == null)
    {
        _logger.LogWarning("MFA verification failed: User not found or inactive. UserId: {UserId}", session.UserId);
        return Result<VerifyMfaCommandResponse>.Fail("User not found or inactive.", ErrorCodes.Auth.UserInactive);
    }

    UserMfa? mfa;

    if (request.IsSetupFlow)
    {
        mfa = await _authRepository.FindPendingMfaByUserIdAsync(user.Id, cancellationToken);
        if (mfa == null)
        {
            _logger.LogWarning("MFA setup verify failed: No pending MFA for UserId: {UserId}", user.Id);
            return Result<VerifyMfaCommandResponse>.Fail("MFA setup not initiated.", ErrorCodes.Auth.TwoFactorSetupRequired);
        }
    }
    else
    {
        mfa = await _authRepository.FindActiveMfaByUserIdAsync(user.Id, cancellationToken);
        if (mfa == null)
        {
            _logger.LogWarning("MFA login verify failed: No active MFA for UserId: {UserId}", user.Id);
            return Result<VerifyMfaCommandResponse>.Fail("MFA not configured for this user.", ErrorCodes.Auth.TwoFactorSetupRequired);
        }
    }

    if (!_mfaService.VerifyMfaCode(mfa.MfaSecret, request.MfaCode))
    {
        _logger.LogWarning("MFA verification failed: Invalid code entered for UserId: {UserId}", user.Id);
        return Result<VerifyMfaCommandResponse>.Fail("Invalid MFA code.", ErrorCodes.Auth.TwoFactorCodeInvalid);
    }

    var tokenConsumed = await _authRepository.MarkMfaSessionTokenAsUsedAsync(session.Id, cancellationToken);
    if (!tokenConsumed)
    {
        _logger.LogWarning("MFA verification race condition detected: Token already consumed. UserId: {UserId}, TokenId: {TokenId}", user.Id, session.Id);
        return Result<VerifyMfaCommandResponse>.Fail("MFA session already used or expired. Please log in again.", ErrorCodes.Auth.MfaTokenAlreadyUsed);
    }

    if (request.IsSetupFlow)
    {
        await _authRepository.VerifyMfaAsync(user.Id, cancellationToken);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("MFA setup completed. UserId: {UserId}", user.Id);

        return Result<VerifyMfaCommandResponse>.Ok(new VerifyMfaCommandResponse
        {
            IsSetupComplete = true
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
        _logger.LogError(ex, "Transaction failed during MFA verification for UserId: {UserId}", user.Id);
        await _unitOfWork.RollbackTransactionAsync();
        throw;
    }

    _logger.LogInformation("MFA verification successful. UserId: {UserId}, Ip: {Ip}", user.Id, request.IpAddress);

    return Result<VerifyMfaCommandResponse>.Ok(new VerifyMfaCommandResponse
    {
        AccessToken = accessToken,
        RefreshToken = refreshToken,
        IsSetupComplete = false
    });
}
}
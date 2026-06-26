using DataAnalysis.Application.Common;
using DataAnalysis.Application.Common.Interfaces;
using DataAnalysis.Application.Common.Settings;
using DataAnalysis.Application.Features.Auth.Abstractions;
using DataAnalysis.Application.Features.Users.Abstractions;
using DataAnalysis.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DataAnalysis.Application.Features.Users.Commands.ResetUserPassword;

public class ResetUserPasswordCommandHandler : IRequestHandler<ResetUserPasswordCommand, Result>
{
    private readonly IUserRepository _userRepository;
    private readonly IAuthRepository _authRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordService _passwordService;
    private readonly IEmailService _emailService;
    private readonly AppSettings _appSettings;
    private readonly EnvironmentSettings _environmentSettings;
    private readonly ILogger<ResetUserPasswordCommandHandler> _logger;

    public ResetUserPasswordCommandHandler(
        IUserRepository userRepository,
        IAuthRepository authRepository,
        IUnitOfWork unitOfWork,
        IPasswordService passwordService,
        IEmailService emailService,
        IOptions<AppSettings> appSettings,
        IOptions<EnvironmentSettings> environmentSettings,
        ILogger<ResetUserPasswordCommandHandler> logger)
    {
        _userRepository = userRepository;
        _authRepository = authRepository;
        _unitOfWork = unitOfWork;
        _passwordService = passwordService;
        _emailService = emailService;
        _appSettings = appSettings.Value;
        _environmentSettings = environmentSettings.Value;
        _logger = logger;
    }

    public async Task<Result> Handle(
        ResetUserPasswordCommand request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Admin attempt to reset password for UserId: {UserId}", request.Id);

        var user = await _userRepository.FindByIdAsync(request.Id, cancellationToken);

        if (user == null)
        {
            _logger.LogWarning("Password reset failed: User with ID {UserId} not found.", request.Id);
            return Result.Fail("User not found.", ErrorCodes.Users.UserNotFound);
        }

        if (!user.IsActive)
        {
            _logger.LogWarning("Password reset aborted: User {UserId} is inactive.", user.Id);
            return Result.Fail("Cannot reset password for an inactive user. Activate the account first.", ErrorCodes.Users.UserInactive);
        }

        var newPassword = _passwordService.GenerateTemporaryPassword();

        user.PasswordHash = _passwordService.HashPassword(newPassword);
        user.IsPasswordChangeRequired = true;
        user.PasswordResetToken = null;
        user.PasswordResetExpiry = null;
        user.FailedLoginAttempts = 0;
        user.LockoutEnd = null;

        try
        {
            await _unitOfWork.BeginTransactionAsync();

            await _unitOfWork.SaveChangesAsync();
            await _authRepository.RevokeAllUserRefreshTokensAsync(user.Id, cancellationToken);

            await _unitOfWork.CommitTransactionAsync();

            _logger.LogInformation("Password reset by admin and refresh tokens revoked for UserId: {UserId}", user.Id);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            _logger.LogError(ex, "Password reset by admin failed, transaction rolled back. UserId: {UserId}", user.Id);
            throw;
        }

        try
        {
            var baseUrl = _environmentSettings.Prod
                ? _appSettings.DefaultUrlProd
                : _appSettings.DefaultUrl;

            var loginLink = $"{baseUrl}/login";
            await _emailService.SendPasswordResetByAdminEmailAsync(
                user.Email,
                $"{user.FirstName} {user.LastName}",
                newPassword,
                loginLink);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Password reset email delivery failed for UserId: {UserId}", user.Id);
        }

        return Result.Ok("Password has been reset and sent to user successfully.");
    }
}
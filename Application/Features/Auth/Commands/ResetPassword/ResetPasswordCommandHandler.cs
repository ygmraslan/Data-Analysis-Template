using DataAnalysis.Application.Common;
using DataAnalysis.Application.Common.Interfaces;
using DataAnalysis.Application.Features.Auth.Abstractions; 
using DataAnalysis.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DataAnalysis.Application.Features.Auth.Commands.ResetPassword;

public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, Result>
{
    private readonly IAuthRepository _authRepository; 
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordService _passwordService;
    private readonly ITokenService _tokenService;
    private readonly ILogger<ResetPasswordCommandHandler> _logger;

    public ResetPasswordCommandHandler(
        IAuthRepository authRepository,
        IUnitOfWork unitOfWork,
        IPasswordService passwordService,
        ITokenService tokenService,
        ILogger<ResetPasswordCommandHandler> logger)
    {
        _authRepository = authRepository;
        _unitOfWork = unitOfWork;
        _passwordService = passwordService;
        _tokenService = tokenService;
        _logger = logger;
    }

    public async Task<Result> Handle(
        ResetPasswordCommand request,
        CancellationToken cancellationToken)
    {
        var tokenHash = _tokenService.HashToken(request.Token);

        var user = await _authRepository.FindByResetTokenAsync(tokenHash, cancellationToken);

        if (user == null)
        {
            _logger.LogWarning("Password reset attempt failed: Invalid, expired token or inactive user.");
            return Result.Fail("Invalid or expired reset token.", ErrorCodes.Auth.InvalidToken);
        }

        if (_passwordService.VerifyPassword(request.NewPassword, user.PasswordHash))
        {
            _logger.LogWarning("Password reset attempt failed: New password is same as old for UserId: {UserId}", user.Id);
            return Result.Fail("New password cannot be the same as the current password.", ErrorCodes.Auth.InvalidCredentials);
        }

        try
        {
            user.PasswordHash = _passwordService.HashPassword(request.NewPassword);
            user.PasswordResetToken = null;
            user.PasswordResetExpiry = null;
            user.IsPasswordChangeRequired = false;

            await _unitOfWork.BeginTransactionAsync();

            await _unitOfWork.SaveChangesAsync();
            await _authRepository.RevokeAllUserRefreshTokensAsync(user.Id, cancellationToken);

            await _unitOfWork.CommitTransactionAsync();

            _logger.LogInformation("Password successfully reset and refresh tokens revoked for UserId: {UserId}", user.Id);
            return Result.Ok("Password has been reset successfully.");
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            _logger.LogError(ex, "An error occurred during password reset for UserId: {UserId}", user.Id);
            throw; 
        }
    }
}
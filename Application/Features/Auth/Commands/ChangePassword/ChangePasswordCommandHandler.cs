using DataAnalysis.Application.Common;
using DataAnalysis.Application.Common.Interfaces;
using DataAnalysis.Application.Features.Auth.Abstractions;
using DataAnalysis.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DataAnalysis.Application.Features.Auth.Commands.ChangePassword;

public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, Result>
{
    private readonly IAuthRepository _authRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordService _passwordService;
    private readonly ITokenService _tokenService;
    private readonly ILogger<ChangePasswordCommandHandler> _logger;

    public ChangePasswordCommandHandler(
        IAuthRepository authRepository,
        IUnitOfWork unitOfWork,
        IPasswordService passwordService,
        ITokenService tokenService,
        ILogger<ChangePasswordCommandHandler> logger)
    {
        _authRepository = authRepository;
        _unitOfWork = unitOfWork;
        _passwordService = passwordService;
        _tokenService = tokenService;
        _logger = logger;
    }

    public async Task<Result> Handle(
        ChangePasswordCommand request,
        CancellationToken cancellationToken)
    {
        int userId;

        if (!string.IsNullOrEmpty(request.PasswordChangeToken))
        {
            var tokenUserId = _tokenService.ValidatePasswordChangeToken(request.PasswordChangeToken);
            if (tokenUserId == null)
            {
                _logger.LogWarning("Password change failed: Invalid or expired password change token.");
                return Result.Fail("Geçersiz veya süresi dolmuş token.", ErrorCodes.Auth.InvalidToken);
            }
            userId = tokenUserId.Value;
        }
        else if (request.AuthenticatedUserId.HasValue)
        {
            userId = request.AuthenticatedUserId.Value;
        }
        else
        {
            _logger.LogWarning("Password change failed: No valid authentication method provided.");
            return Result.Fail("Yetkisiz erişim.", ErrorCodes.Auth.InvalidToken);
        }

        var user = await _authRepository.FindByIdAsync(userId, cancellationToken);

        if (user == null)
        {
            _logger.LogWarning("Password change failed: User {UserId} not found or inactive.", userId);
            return Result.Fail("User not found or inactive.", ErrorCodes.Auth.UserInactive);
        }

        if (!_passwordService.VerifyPassword(request.CurrentPassword, user.PasswordHash))
        {
            _logger.LogWarning("Password change failed: Incorrect current password for User {UserId}.", user.Id);
            return Result.Fail("Current password is incorrect.", ErrorCodes.Auth.CurrentPasswordIncorrect);
        }

        if (_passwordService.VerifyPassword(request.NewPassword, user.PasswordHash))
        {
            _logger.LogWarning("Password change failed: New password is same as current for User {UserId}.", user.Id);
            return Result.Fail("New password cannot be the same as the current password.", ErrorCodes.Auth.SamePassword);
        }

        try
        {
            user.PasswordHash = _passwordService.HashPassword(request.NewPassword);
            user.IsPasswordChangeRequired = false;

            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Password successfully changed for User {UserId}.", user.Id);
            return Result.Ok("Password changed successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while changing password for User {UserId}.", user.Id);
            throw;
        }
    }
}
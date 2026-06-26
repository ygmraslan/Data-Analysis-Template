using DataAnalysis.Application.Common;
using DataAnalysis.Application.Features.Users.Abstractions; 
using DataAnalysis.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DataAnalysis.Application.Features.Users.Commands.ResetUserMfa;

public class ResetUserMfaCommandHandler : IRequestHandler<ResetUserMfaCommand, Result>
{
    private readonly IUserRepository _userRepository; 
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ResetUserMfaCommandHandler> _logger;

    public ResetUserMfaCommandHandler(
        IUserRepository userRepository, 
        IUnitOfWork unitOfWork, 
        ILogger<ResetUserMfaCommandHandler> logger)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(
        ResetUserMfaCommand request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Admin attempt to reset MFA for UserId: {UserId}", request.Id);

        var user = await _userRepository.FindByIdAsync(request.Id, cancellationToken);

        if (user == null)
        {
            _logger.LogWarning("MFA reset failed: User with ID {UserId} not found.", request.Id);
            return Result.Fail("User not found.", ErrorCodes.Users.UserNotFound);
        }

        var mfa = await _userRepository.FindMfaByUserIdAsync(user.Id, cancellationToken);

        if (mfa == null)
        {
            _logger.LogWarning("MFA reset failed: User {Email} does not have MFA configured.", user.Email);
            return Result.Fail("User does not have MFA configured.", ErrorCodes.Users.MfaNotConfigured);
        }

        try
        {
            mfa.IsEnabled = false;
            mfa.IsVerified = false;
            mfa.MfaSecret = string.Empty;

            await _unitOfWork.SaveChangesAsync(); 

            _logger.LogInformation("MFA has been successfully reset for User: {Email} by Admin.", user.Email);
            return Result.Ok("MFA has been reset successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while resetting MFA for User {Email}.", user.Email);
            throw;
        }
    }
}
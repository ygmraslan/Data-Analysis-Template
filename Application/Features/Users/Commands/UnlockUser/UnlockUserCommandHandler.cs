using DataAnalysis.Application.Common;
using DataAnalysis.Application.Features.Users.Abstractions;
using DataAnalysis.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DataAnalysis.Application.Features.Users.Commands.UnlockUser;

public class UnlockUserCommandHandler : IRequestHandler<UnlockUserCommand, Result>
{
    private readonly IUserRepository _userRepository; 
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UnlockUserCommandHandler> _logger;

    public UnlockUserCommandHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        ILogger<UnlockUserCommandHandler> logger)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(
        UnlockUserCommand request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Attempting to unlock account for User {UserId}", request.Id);

        var user = await _userRepository.FindByIdAsync(request.Id, cancellationToken);

        if (user == null)
        {
            _logger.LogWarning("Unlock failed: User {UserId} not found.", request.Id);
            return Result.Fail("User not found.", ErrorCodes.Users.UserNotFound);
        }

        if (!user.LockoutEnd.HasValue || user.LockoutEnd <= DateTime.UtcNow)
        {
            _logger.LogInformation("Unlock aborted: User {UserId} is not currently locked.", user.Id);
            return Result.Fail("User account is not locked.", ErrorCodes.Users.UserNotLocked);
        }

        try
        {
            user.LockoutEnd = null;
            user.FailedLoginAttempts = 0;

            await _unitOfWork.SaveChangesAsync(); 

            _logger.LogInformation("Account for User {UserId} has been successfully unlocked.", user.Id);
            return Result.Ok("User account unlocked successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while unlocking account for User {UserId}.", request.Id);
            throw; 
        }
    }
}
using DataAnalysis.Application.Common;
using DataAnalysis.Application.Features.Users.Abstractions;
using DataAnalysis.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging; 

namespace DataAnalysis.Application.Features.Users.Commands.ToggleUserStatus;

public class ToggleUserStatusCommandHandler : IRequestHandler<ToggleUserStatusCommand, Result<bool>>
{
    private readonly IUserRepository _userRepository; 
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ToggleUserStatusCommandHandler> _logger;

    public ToggleUserStatusCommandHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        ILogger<ToggleUserStatusCommandHandler> logger)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<bool>> Handle(
        ToggleUserStatusCommand request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Toggling activation status for User {UserId}", request.Id);

        var user = await _userRepository.FindByIdAsync(request.Id, cancellationToken);

        if (user == null)
        {
            _logger.LogWarning("Status toggle failed: User {UserId} not found.", request.Id);
            return Result<bool>.Fail("User not found.", ErrorCodes.Users.UserNotFound);
        }

        try
        {
            user.IsActive = !user.IsActive;

            await _unitOfWork.SaveChangesAsync(); 

            var statusMessage = user.IsActive ? "User activated successfully." : "User deactivated successfully.";
            _logger.LogInformation("User {UserId} status successfully updated to {IsActive}.", user.Id, user.IsActive);

            return Result<bool>.Ok(user.IsActive, statusMessage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while toggling status for User {UserId}.", request.Id);
            throw;
        }
    }
}
using DataAnalysis.Application.Common;
using DataAnalysis.Application.Features.Users.Abstractions; 
using DataAnalysis.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DataAnalysis.Application.Features.Users.Commands.UpdateUser;

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, Result>
{
    private readonly IUserRepository _userRepository; 
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateUserCommandHandler> _logger;

    public UpdateUserCommandHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        ILogger<UpdateUserCommandHandler> logger)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(
        UpdateUserCommand request,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.FindByIdAsync(request.Id, cancellationToken);

        if (user == null)
        {
            _logger.LogWarning("Update failed: User {UserId} not found.", request.Id);
            return Result.Fail("User not found.", ErrorCodes.Users.UserNotFound);
        }

        var emailExists = await _userRepository.EmailExistsAsync(request.Email, request.Id, cancellationToken);

        if (emailExists)
        {
            _logger.LogWarning("Update failed: Email {Email} is already taken by another user.", request.Email);
            return Result.Fail("A user with this email already exists.", ErrorCodes.Users.EmailAlreadyExists);
        }

        try
        {
            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.Email = request.Email;

            await _unitOfWork.SaveChangesAsync(); 

            _logger.LogInformation("User {UserId} updated successfully.", user.Id);
            return Result.Ok("User updated successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while updating User {UserId}.", request.Id);
            throw; 
        }
    }
}
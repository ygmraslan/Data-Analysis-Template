using DataAnalysis.Application.Common;
using DataAnalysis.Application.Features.Permissions.Abstractions;
using DataAnalysis.Application.Features.Users.Abstractions;
using DataAnalysis.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DataAnalysis.Application.Features.Permissions.Commands.AssignPermissions;

public class AssignPermissionsCommandHandler : IRequestHandler<AssignPermissionsCommand, Result>
{
    private readonly IPermissionRepository _permissionRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AssignPermissionsCommandHandler> _logger;

    public AssignPermissionsCommandHandler(
        IPermissionRepository permissionRepository,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        ILogger<AssignPermissionsCommandHandler> logger)
    {
        _permissionRepository = permissionRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(
        AssignPermissionsCommand request,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.FindByIdAsNoTrackingAsync(request.UserId, cancellationToken);

        if (user == null)
        {
            _logger.LogWarning("Permission assignment failed: User {UserId} not found.", request.UserId);
            return Result.Fail("User not found.", ErrorCodes.Users.UserNotFound);
        }

        try
        {
            await _unitOfWork.BeginTransactionAsync();
            await _permissionRepository.SyncUserPermissionsAsync(request.UserId, request.PermissionIds, cancellationToken);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Permission assignment failed for UserId: {UserId}", request.UserId);
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }

        _logger.LogInformation("Permissions successfully synced for UserId: {UserId}", request.UserId);
        return Result.Ok("Permissions updated successfully.");
    }
}
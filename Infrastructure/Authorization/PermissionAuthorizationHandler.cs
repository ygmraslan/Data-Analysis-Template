using DataAnalysis.Application.Features.Permissions.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;

namespace DataAnalysis.Infrastructure.Authorization;

public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly IServiceScopeFactory _scopeFactory;

    public PermissionAuthorizationHandler(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

   protected override async Task HandleRequirementAsync(
    AuthorizationHandlerContext context,
    PermissionRequirement requirement)
    {
        var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userIdClaim == null || !int.TryParse(userIdClaim, out var userId))
            return;

        using var scope = _scopeFactory.CreateScope();
        var permissionRepository = scope.ServiceProvider.GetRequiredService<IPermissionRepository>();

        var hasPermission = await permissionRepository.UserHasPermissionAsync(userId, requirement.Permission);

        if (hasPermission)
            context.Succeed(requirement);
    }
}
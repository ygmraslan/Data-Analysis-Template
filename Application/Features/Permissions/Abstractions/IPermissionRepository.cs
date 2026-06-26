using DataAnalysis.Domain.Entities.Permission;

namespace DataAnalysis.Application.Features.Permissions.Abstractions;

public interface IPermissionRepository
{
    Task<List<Module>> GetModulesWithPermissionsAsync(CancellationToken ct = default);
    Task<List<int>> GetUserPermissionIdsAsync(int userId, CancellationToken ct = default);
    Task SyncUserPermissionsAsync(int userId, List<int> permissionIds, CancellationToken ct = default);
    Task<bool> UserHasPermissionAsync(int userId, string permissionName, CancellationToken ct = default);
    Task<(string Module, string Action, string ActionDescription)?> FindByPolicyNameAsync(string policyName, CancellationToken ct = default);
}
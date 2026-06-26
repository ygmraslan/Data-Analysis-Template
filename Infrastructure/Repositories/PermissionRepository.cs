using DataAnalysis.Application.Features.Permissions.Abstractions;
using DataAnalysis.Domain.Entities.Permission;
using DataAnalysis.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace DataAnalysis.Infrastructure.Repositories;

public class PermissionRepository : IPermissionRepository
{
    private readonly AppDbContext _context;

    public PermissionRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Module>> GetModulesWithPermissionsAsync(CancellationToken ct = default)
        => await _context.Modules
            .Include(m => m.Permissions.Where(p => p.IsActive))
            .AsNoTracking()
            .ToListAsync(ct);

    public async Task<List<int>> GetUserPermissionIdsAsync(int userId, CancellationToken ct = default)
        => await _context.UserPermissions
            .Where(up => up.UserId == userId && up.DeletedDate == null)
            .Select(up => up.PermissionId)
            .ToListAsync(ct);        

    public async Task SyncUserPermissionsAsync(int userId, List<int> permissionIds, CancellationToken ct = default)
    {
        var existingPermissions = await _context.UserPermissions
            .Where(up => up.UserId == userId)
            .ToListAsync(ct);

        foreach (var up in existingPermissions.Where(up => up.DeletedDate == null && !permissionIds.Contains(up.PermissionId)))
            up.DeletedDate = DateTime.UtcNow;

        foreach (var up in existingPermissions.Where(up => up.DeletedDate != null && permissionIds.Contains(up.PermissionId)))
            up.DeletedDate = null;

        var existingPermissionIds = existingPermissions.Select(up => up.PermissionId).ToList();
        var toAdd = permissionIds
            .Where(id => !existingPermissionIds.Contains(id))
            .Select(id => new UserPermission
            {
                UserId = userId,
                PermissionId = id,
                CreatedDate = DateTime.UtcNow
            });

        await _context.UserPermissions.AddRangeAsync(toAdd, ct);
    }

    public async Task<bool> UserHasPermissionAsync(int userId, string permissionName, CancellationToken ct = default)
    => await _context.UserPermissions
        .Where(up => up.UserId == userId && up.DeletedDate == null)
        .Join(_context.Permissions,
            up => up.PermissionId,
            p => p.Id,
            (up, p) => p.Name)
        .AnyAsync(name => name == permissionName, ct);

    public async Task<(string Module, string Action, string ActionDescription)?> FindByPolicyNameAsync(string policyName, CancellationToken ct = default)
    {
        var result = await _context.Permissions
            .AsNoTracking()
            .Where(p => p.Name == policyName && p.IsActive)
            .Select(p => new { p.Module.Name, p.Description, PermissionName = p.Name })
            .FirstOrDefaultAsync(ct);
 
        if (result == null) return null;
 
        return (result.Name, result.PermissionName, result.Description ?? result.PermissionName);
    }  
}
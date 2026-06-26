using DataAnalysis.Application.Features.Users.Abstractions;
using DataAnalysis.Domain.Entities.Identity;
using DataAnalysis.Domain.Entities.Permission;
using DataAnalysis.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace DataAnalysis.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public IQueryable<User> GetUsersQuery()
        => _context.Set<User>().AsNoTracking();

    public async Task<User?> FindByIdAsync(int id, CancellationToken ct = default)
        => await _context.Set<User>()
            .FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task<User?> FindByIdAsNoTrackingAsync(int id, CancellationToken ct = default)
    => await _context.Set<User>()
        .AsNoTracking()
        .FirstOrDefaultAsync(x => x.Id == id, ct);        

    public async Task<bool> EmailExistsAsync(string email, int? excludeUserId = null, CancellationToken ct = default)
        => await _context.Set<User>()
            .AsNoTracking()
            .AnyAsync(x => x.Email.ToLower() == email.Trim().ToLower()
                && (!excludeUserId.HasValue || x.Id != excludeUserId.Value), ct);

    public async Task<UserMfa?> FindMfaByUserIdAsync(int userId, CancellationToken ct = default)
        => await _context.Set<UserMfa>()
            .FirstOrDefaultAsync(x => x.UserId == userId, ct);

    public async Task<bool> HasActiveMfaAsync(int userId, CancellationToken ct = default)
        => await _context.Set<UserMfa>()
            .AsNoTracking()
            .AnyAsync(x => x.UserId == userId && x.IsEnabled && x.IsVerified, ct);

    public async Task AddMfaAsync(UserMfa userMfa, CancellationToken ct = default)
    => await _context.Set<UserMfa>().AddAsync(userMfa, ct);        

    public async Task<List<int>> GetActiveMfaUserIdsAsync(List<int> userIds, CancellationToken ct = default)
    => await _context.Set<UserMfa>()
        .AsNoTracking()
        .Where(x => userIds.Contains(x.UserId) && x.IsEnabled && x.IsVerified)
        .Select(x => x.UserId)
        .ToListAsync(ct);        

    public async Task AddAsync(User user, CancellationToken ct = default)
        => await _context.Set<User>().AddAsync(user, ct);

    public async Task<List<string>> GetUserPermissionsAsync(int userId, CancellationToken ct = default)
    => await _context.Set<UserPermission>()
        .AsNoTracking()
        .Where(x => x.UserId == userId && x.DeletedDate == null)
        .Select(x => x.Permission.Name)
        .ToListAsync(ct);    
}
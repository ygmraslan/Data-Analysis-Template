using DataAnalysis.Domain.Entities.Identity;

namespace DataAnalysis.Application.Features.Users.Abstractions;

public interface IUserRepository
{
    // Queries
    IQueryable<User> GetUsersQuery();
    Task<User?> FindByIdAsync(int id, CancellationToken ct = default);
    Task<User?> FindByIdAsNoTrackingAsync(int id, CancellationToken ct = default);
    Task<bool> EmailExistsAsync(string email, int? excludeUserId = null, CancellationToken ct = default);

    // MFA
    Task<UserMfa?> FindMfaByUserIdAsync(int userId, CancellationToken ct = default);
    Task<bool> HasActiveMfaAsync(int userId, CancellationToken ct = default);
    Task<List<int>> GetActiveMfaUserIdsAsync(List<int> userIds, CancellationToken ct = default);
    Task AddMfaAsync(UserMfa userMfa, CancellationToken ct = default);

    // Commands
    Task AddAsync(User user, CancellationToken ct = default);
    Task<List<string>> GetUserPermissionsAsync(int userId, CancellationToken ct = default);
}
using DataAnalysis.Application.Features.Auth.Abstractions;
using DataAnalysis.Domain.Entities.Identity;
using DataAnalysis.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace DataAnalysis.Infrastructure.Repositories;

public class AuthRepository : IAuthRepository
{
    private readonly AppDbContext _context;

    public AuthRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<User?> FindByEmailAsync(string email, CancellationToken ct = default)
        => await _context.Set<User>()
            .FirstOrDefaultAsync(x => x.Email.ToLower() == email.Trim().ToLower() && x.IsActive, ct);

    public async Task<User?> FindByIdAsync(int id, CancellationToken ct = default)
        => await _context.Set<User>()
            .FirstOrDefaultAsync(x => x.Id == id && x.IsActive, ct);

    public async Task UpdatePasswordAsync(int userId, string passwordHash, CancellationToken ct = default)
        => await _context.Set<User>()
            .Where(x => x.Id == userId)
            .ExecuteUpdateAsync(x => x
                .SetProperty(u => u.PasswordHash, passwordHash)
                .SetProperty(u => u.IsPasswordChangeRequired, false), ct);

    public async Task ResetFailedLoginAttemptsAsync(int userId, CancellationToken ct = default)
        => await _context.Set<User>()
            .Where(x => x.Id == userId)
            .ExecuteUpdateAsync(x => x
                .SetProperty(u => u.FailedLoginAttempts, 0)
                .SetProperty(u => u.LockoutEnd, (DateTime?)null), ct);
    public async Task UpdateLastLoginDateAsync(int userId, CancellationToken ct = default)
    => await _context.Set<User>()
        .Where(x => x.Id == userId)
        .ExecuteUpdateAsync(x => x
            .SetProperty(u => u.LastLoginDate, DateTime.UtcNow), ct);            
    public async Task<UserMfa?> FindPendingMfaByUserIdAsync(int userId, CancellationToken ct = default)
        => await _context.Set<UserMfa>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.UserId == userId && x.IsEnabled && !x.IsVerified, ct);
    public async Task<UserMfa?> FindResetMfaByUserIdAsync(int userId, CancellationToken ct = default)
        => await _context.Set<UserMfa>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.UserId == userId && !x.IsEnabled && !x.IsVerified, ct);

    public async Task<UserMfa?> FindActiveMfaByUserIdAsync(int userId, CancellationToken ct = default)
        => await _context.Set<UserMfa>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.UserId == userId && x.IsEnabled && x.IsVerified, ct);

    public async Task AddMfaAsync(UserMfa userMfa, CancellationToken ct = default)
    => await _context.UserMfas.AddAsync(userMfa, ct);        

    public async Task UpdateMfaSecretAsync(int userId, string secret, CancellationToken ct = default)
        => await _context.Set<UserMfa>()
            .Where(x => x.UserId == userId)
            .ExecuteUpdateAsync(x => x
                .SetProperty(u => u.MfaSecret, secret)
                .SetProperty(u => u.IsVerified, false)
                .SetProperty(u => u.IsEnabled, true), ct);

    public async Task VerifyMfaAsync(int userId, CancellationToken ct = default)
    => await _context.Set<UserMfa>()
        .Where(x => x.UserId == userId)
        .ExecuteUpdateAsync(x => x
            .SetProperty(u => u.IsVerified, true), ct);            
    public async Task AddMfaSessionTokenAsync(MfaSessionToken token, CancellationToken ct = default)
        => await _context.Set<MfaSessionToken>().AddAsync(token, ct);

    public async Task<MfaSessionToken?> FindActiveMfaSessionTokenAsync(string tokenHash, CancellationToken ct = default)
        => await _context.Set<MfaSessionToken>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Token == tokenHash
                && x.UsedDate == null
                && x.ExpiresDate > DateTime.UtcNow, ct);

    public async Task<bool> MarkMfaSessionTokenAsUsedAsync(int id, CancellationToken ct = default)
        => await _context.Set<MfaSessionToken>()
            .Where(x => x.Id == id && x.UsedDate == null)
            .ExecuteUpdateAsync(x => x
                .SetProperty(t => t.UsedDate, DateTime.UtcNow), ct) > 0;

    public async Task AddRefreshTokenAsync(RefreshToken token, CancellationToken ct = default)
        => await _context.Set<RefreshToken>().AddAsync(token, ct);

    public async Task<RefreshToken?> FindValidRefreshTokenAsync(string tokenHash, CancellationToken ct = default)
        => await _context.Set<RefreshToken>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Token == tokenHash
                && x.RevokedDate == null
                && x.ExpiresDate > DateTime.UtcNow, ct);

    public async Task RevokeRefreshTokenAsync(RefreshToken token, CancellationToken ct = default)
    {
        var tracked = await _context.Set<RefreshToken>()
            .FirstOrDefaultAsync(x => x.Id == token.Id, ct);

        if (tracked == null) return;

        tracked.RevokedDate = DateTime.UtcNow;
        _context.Set<RefreshToken>().Update(tracked);
    }

    public async Task RevokeAllUserRefreshTokensAsync(int userId, CancellationToken ct = default)
        => await _context.Set<RefreshToken>()
            .Where(x => x.UserId == userId && x.RevokedDate == null)
            .ExecuteUpdateAsync(x => x
                .SetProperty(t => t.RevokedDate, DateTime.UtcNow), ct);

    public async Task<User?> FindByResetTokenAsync(string tokenHash, CancellationToken ct = default)
        => await _context.Set<User>()
            .FirstOrDefaultAsync(x => x.PasswordResetToken == tokenHash
                && x.IsActive
                && x.PasswordResetExpiry > DateTime.UtcNow, ct);

    public async Task ClearResetTokenAsync(int userId, CancellationToken ct = default)
        => await _context.Set<User>()
            .Where(x => x.Id == userId)
            .ExecuteUpdateAsync(x => x
                .SetProperty(u => u.PasswordResetToken, (string?)null)
                .SetProperty(u => u.PasswordResetExpiry, (DateTime?)null), ct);
}
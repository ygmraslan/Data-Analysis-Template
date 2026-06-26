using DataAnalysis.Domain.Entities.Identity;

namespace DataAnalysis.Application.Features.Auth.Abstractions;

public interface IAuthRepository
{
    // User
    Task<User?> FindByEmailAsync(string email, CancellationToken ct = default);
    Task<User?> FindByIdAsync(int id, CancellationToken ct = default);
    Task UpdatePasswordAsync(int userId, string passwordHash, CancellationToken ct = default);
    Task UpdateLastLoginDateAsync(int userId, CancellationToken ct = default);
    Task ResetFailedLoginAttemptsAsync(int userId, CancellationToken ct = default);

    // MFA
    Task<UserMfa?> FindPendingMfaByUserIdAsync(int userId, CancellationToken ct = default);
    Task<UserMfa?> FindResetMfaByUserIdAsync(int userId, CancellationToken ct = default);
    Task<UserMfa?> FindActiveMfaByUserIdAsync(int userId, CancellationToken ct = default);
    Task AddMfaAsync(UserMfa userMfa, CancellationToken ct = default);
    Task UpdateMfaSecretAsync(int userId, string secret, CancellationToken ct = default);
    Task VerifyMfaAsync(int userId, CancellationToken ct = default);

    // MfaSessionToken
    Task AddMfaSessionTokenAsync(MfaSessionToken token, CancellationToken ct = default);
    Task<MfaSessionToken?> FindActiveMfaSessionTokenAsync(string tokenHash, CancellationToken ct = default);
    Task<bool> MarkMfaSessionTokenAsUsedAsync(int id, CancellationToken ct = default);

    // RefreshToken
    Task AddRefreshTokenAsync(RefreshToken token, CancellationToken ct = default);
    Task<RefreshToken?> FindValidRefreshTokenAsync(string tokenHash, CancellationToken ct = default);
    Task RevokeRefreshTokenAsync(RefreshToken token, CancellationToken ct = default);
    Task RevokeAllUserRefreshTokensAsync(int userId, CancellationToken ct = default);

    // PasswordResetToken
    Task<User?> FindByResetTokenAsync(string tokenHash, CancellationToken ct = default);
    Task ClearResetTokenAsync(int userId, CancellationToken ct = default);
    
}
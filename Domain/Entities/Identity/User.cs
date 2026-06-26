using DataAnalysis.Domain.Common;
using DataAnalysis.Domain.Entities.Logging;
using DataAnalysis.Domain.Entities.Permission;

namespace DataAnalysis.Domain.Entities.Identity;

public class User : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public bool IsPasswordChangeRequired { get; set; }
    public DateTime? LastLoginDate { get; set; }
    public string? PasswordResetToken { get; set; }
    public DateTime? PasswordResetExpiry { get; set; }
    public int FailedLoginAttempts { get; set; }
    public DateTime? LockoutEnd { get; set; }   

    public UserMfa? UserMfa { get; set; }
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    public ICollection<UserPermission> UserPermissions { get; set; } = new List<UserPermission>();
    public ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();
}
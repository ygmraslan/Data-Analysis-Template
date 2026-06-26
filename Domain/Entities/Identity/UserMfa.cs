
namespace DataAnalysis.Domain.Entities.Identity;

public class UserMfa
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string MfaSecret { get; set; } = string.Empty;
    public bool IsVerified { get; set; }
    public bool IsEnabled { get; set; }
    public DateTime? ResetRequestedDate { get; set; }
    public int? ResetRequestedBy { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public User User { get; set; } = null!;
}
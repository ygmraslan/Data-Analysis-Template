namespace DataAnalysis.Domain.Entities.Identity;

public class MfaSessionToken
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresDate { get; set; }
    public DateTime? UsedDate { get; set; }
    public string IpAddress { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public User User { get; set; } = null!;
}
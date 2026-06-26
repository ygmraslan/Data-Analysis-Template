namespace DataAnalysis.Domain.Entities.Identity;

public class RefreshToken
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresDate { get; set; }
    public DateTime? RevokedDate { get; set; }
    public string IpAddress { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public User User { get; set; } = null!;
}
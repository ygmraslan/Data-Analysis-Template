using DataAnalysis.Domain.Entities.Identity;

namespace DataAnalysis.Domain.Entities.Logging;

public class AuthLog
{
    public int Id { get; set; }
    public int? UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public bool Success { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;
    public string Browser { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    public User? User { get; set; }
}
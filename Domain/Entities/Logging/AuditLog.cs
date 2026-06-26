using DataAnalysis.Domain.Entities.Identity;

namespace DataAnalysis.Domain.Entities.Logging;

public class AuditLog
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Module { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string ActionDescription { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;
    public string Browser { get; set; } = string.Empty;
    public string Method { get; set; } = string.Empty;
    public string Request { get; set; } = string.Empty;
    public int StatusCode { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    public User User { get; set; } = null!;
}
namespace DataAnalysis.Application.Features.AuthLogs.Dtos;

public class AuthLogDto
{
    public int Id { get; set; }
    public int? UserId { get; set; }
     public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool Success { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;
    public string Browser { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
}
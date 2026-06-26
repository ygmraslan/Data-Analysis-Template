namespace DataAnalysis.Application.Features.AuditLogs.Dtos;

public class AuditLogDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Module { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string ActionDescription { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;
    public string Browser { get; set; } = string.Empty;
    public string Method { get; set; } = string.Empty;
    public string Request { get; set; } = string.Empty;
    public int StatusCode { get; set; }
    public DateTime CreatedDate { get; set; }
}
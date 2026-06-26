using DataAnalysis.Application.Features.AuditLogs.Dtos;

namespace DataAnalysis.Application.Features.AuditLogs.Queries.GetAuditLogs;

public class GetAuditLogsQueryResponse
{
    public List<AuditLogDto> Logs { get; set; } = new();
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
}
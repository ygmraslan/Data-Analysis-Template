using DataAnalysis.Application.Common;
using MediatR;

namespace DataAnalysis.Application.Features.AuditLogs.Queries.GetAuditLogs;

public class GetAuditLogsQuery : IRequest<Result<GetAuditLogsQueryResponse>>
{
    public int? UserId { get; set; }
    public string? Module { get; set; }
    public string? Action { get; set; }
    public int? StatusCode { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
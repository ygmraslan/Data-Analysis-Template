using DataAnalysis.Application.Common;
using MediatR;

namespace DataAnalysis.Application.Features.AuthLogs.Queries.GetAuthLogs;

public class GetAuthLogsQuery : IRequest<Result<GetAuthLogsQueryResponse>>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? Email { get; set; }
    public bool? Success { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}
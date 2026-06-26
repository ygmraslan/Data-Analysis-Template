using DataAnalysis.Application.Features.AuthLogs.Dtos;

namespace DataAnalysis.Application.Features.AuthLogs.Queries.GetAuthLogs;

public class GetAuthLogsQueryResponse
{
    public List<AuthLogDto> Logs { get; set; } = [];
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
}
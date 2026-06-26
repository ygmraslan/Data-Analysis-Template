using DataAnalysis.Application.Features.ExecSummary.Dtos;

namespace DataAnalysis.Application.Features.ExecSummary.Queries.GetYoungDriver;

public class GetYoungDriverQueryResponse
{
    public List<DistributionItemDto> Brands { get; set; } = new();
}
using DataAnalysis.Application.Features.ExecSummary.Dtos;

namespace DataAnalysis.Application.Features.ExecSummary.Queries.GetRisk;

public class GetRiskQueryResponse
{
    public List<RiskSegmentDto> Segments { get; set; } = new();
}
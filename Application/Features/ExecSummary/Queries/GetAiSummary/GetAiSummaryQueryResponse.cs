using DataAnalysis.Application.Features.ExecSummary.Dtos;

namespace DataAnalysis.Application.Features.ExecSummary.Queries.GetAiSummary;

public class GetAiSummaryQueryResponse
{
    public ModelResponseDto Data { get; set; } = new();
    public bool FromCache { get; set; }
}
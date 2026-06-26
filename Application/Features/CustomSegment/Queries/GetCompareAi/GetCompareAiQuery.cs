using DataAnalysis.Application.Common.Enums;
using DataAnalysis.Application.Features.CustomSegment.Dtos;
using MediatR;

namespace DataAnalysis.Application.Features.CustomSegment.Queries.GetCompareAi;

public class GetCompareAiQuery : IRequest<GetCompareAiResponse>
{
    public string ProductGroup { get; set; } = string.Empty;
    public AiModelType ModelType { get; set; }
    public DateTime WeekStart { get; set; }
    public DateTime WeekEnd { get; set; }

    public SegmentFilterDto? FiltersA { get; set; }
    public SegmentDriftResultDto ResultA { get; set; } = new();

    public SegmentFilterDto? FiltersB { get; set; }
    public SegmentDriftResultDto ResultB { get; set; } = new();
}
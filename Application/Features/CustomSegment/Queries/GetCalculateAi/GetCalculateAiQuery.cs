using DataAnalysis.Application.Common.Enums;
using DataAnalysis.Application.Features.CustomSegment.Dtos;
using MediatR;

namespace DataAnalysis.Application.Features.CustomSegment.Queries.GetCalculateAi;

public class GetCalculateAiQuery : IRequest<GetCalculateAiQueryResponse>
{
    public string ProductGroup { get; set; } = string.Empty;
    public AiModelType ModelType { get; set; }
    public DateTime WeekStart { get; set; }
    public DateTime WeekEnd { get; set; }
    public SegmentFilterDto? Filters { get; set; }
    public SegmentDriftResultDto Result { get; set; } = new();
}
using DataAnalysis.Application.Common.Enums;
using MediatR;

namespace DataAnalysis.Application.Features.ExecSummary.Queries.GetAiSummary;

public class GetAiSummaryQuery : IRequest<GetAiSummaryQueryResponse>
{
    public ProductGroup ProductGroup { get; set; }
    public AiModelType ModelType { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool ForceRefresh { get; set; }
    public int? UserId { get; set; }
}
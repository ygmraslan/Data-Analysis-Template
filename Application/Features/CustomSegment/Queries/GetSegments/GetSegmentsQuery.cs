using DataAnalysis.Application.Common.Enums;
using MediatR;

namespace DataAnalysis.Application.Features.CustomSegment.Queries.GetSegments;

public class GetSegmentsQuery : IRequest<GetSegmentsQueryResponse>
{
    public ProductGroup? ProductGroup { get; set; }
    public string? Search { get; set; }
}
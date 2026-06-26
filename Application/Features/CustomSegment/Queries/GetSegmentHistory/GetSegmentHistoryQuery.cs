using MediatR;

namespace DataAnalysis.Application.Features.CustomSegment.Queries.GetSegmentHistory;

public class GetSegmentHistoryQuery : IRequest<GetSegmentHistoryResponse>
{
    public int SegmentId { get; set; }
}
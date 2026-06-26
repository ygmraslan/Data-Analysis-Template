using MediatR;

namespace DataAnalysis.Application.Features.CustomSegment.Queries.GetSegmentById;

public class GetSegmentByIdQuery : IRequest<GetSegmentByIdQueryResponse>
{
    public int Id { get; set; }
}
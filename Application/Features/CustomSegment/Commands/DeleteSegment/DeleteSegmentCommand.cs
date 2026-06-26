using MediatR;

namespace DataAnalysis.Application.Features.CustomSegment.Commands.DeleteSegment;

public class DeleteSegmentCommand : IRequest<DeleteSegmentResponse>
{
    public int SegmentId { get; set; }
    public int UserId { get; set; }
}
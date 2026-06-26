using MediatR;

namespace DataAnalysis.Application.Features.CustomSegment.Commands.RunSegment;

public class RunSegmentCommand : IRequest<RunSegmentResponse>
{
    public int SegmentId { get; set; }
    public DateTime WeekStart { get; set; }
    public DateTime WeekEnd { get; set; }
    public int? UserId { get; set; }
}
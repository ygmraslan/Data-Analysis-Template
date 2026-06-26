using MediatR;

namespace DataAnalysis.Application.Features.CustomSegment.Commands.RunComparison;

public class RunComparisonCommand : IRequest<RunComparisonResponse>
{
    public int ComparisonId { get; set; }
    public DateTime WeekStart { get; set; }
    public DateTime WeekEnd { get; set; }
    public int UserId { get; set; }
}
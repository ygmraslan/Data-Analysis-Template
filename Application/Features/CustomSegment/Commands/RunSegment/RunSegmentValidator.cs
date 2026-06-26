using FluentValidation;

namespace DataAnalysis.Application.Features.CustomSegment.Commands.RunSegment;

public class RunSegmentValidator : AbstractValidator<RunSegmentCommand>
{
    public RunSegmentValidator()
    {
        RuleFor(x => x.SegmentId).GreaterThan(0);
        RuleFor(x => x.WeekStart).NotEmpty();
        RuleFor(x => x.WeekEnd).NotEmpty().GreaterThan(x => x.WeekStart);
    }
}
using FluentValidation;

namespace DataAnalysis.Application.Features.CustomSegment.Commands.RunComparison;

public class RunComparisonValidator : AbstractValidator<RunComparisonCommand>
{
    public RunComparisonValidator()
    {
        RuleFor(x => x.ComparisonId).GreaterThan(0);
        RuleFor(x => x.WeekStart).NotEmpty();
        RuleFor(x => x.WeekEnd).NotEmpty().GreaterThan(x => x.WeekStart);
    }
}
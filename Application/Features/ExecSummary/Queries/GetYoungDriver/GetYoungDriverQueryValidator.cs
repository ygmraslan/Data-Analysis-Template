using FluentValidation;

namespace DataAnalysis.Application.Features.ExecSummary.Queries.GetYoungDriver;

public class GetYoungDriverQueryValidator : AbstractValidator<GetYoungDriverQuery>
{
    public GetYoungDriverQueryValidator()
    {
        RuleFor(x => x.ProductGroup).IsInEnum();
        RuleFor(x => x.StartDate).NotEmpty();
        RuleFor(x => x.EndDate).NotEmpty().GreaterThanOrEqualTo(x => x.StartDate);
    }
}
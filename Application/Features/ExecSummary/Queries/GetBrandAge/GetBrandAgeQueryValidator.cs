using FluentValidation;

namespace DataAnalysis.Application.Features.ExecSummary.Queries.GetBrandAge;

public class GetBrandAgeQueryValidator : AbstractValidator<GetBrandAgeQuery>
{
    public GetBrandAgeQueryValidator()
    {
        RuleFor(x => x.ProductGroup).IsInEnum();
        RuleFor(x => x.StartDate).NotEmpty();
        RuleFor(x => x.EndDate).NotEmpty().GreaterThanOrEqualTo(x => x.StartDate);
    }
}
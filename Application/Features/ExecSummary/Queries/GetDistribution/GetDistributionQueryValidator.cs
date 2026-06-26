using FluentValidation;

namespace DataAnalysis.Application.Features.ExecSummary.Queries.GetDistribution;

public class GetDistributionQueryValidator : AbstractValidator<GetDistributionQuery>
{
    public GetDistributionQueryValidator()
    {
        RuleFor(x => x.ProductGroup).IsInEnum();
        RuleFor(x => x.StartDate).NotEmpty();
        RuleFor(x => x.EndDate).NotEmpty().GreaterThanOrEqualTo(x => x.StartDate);
    }
}
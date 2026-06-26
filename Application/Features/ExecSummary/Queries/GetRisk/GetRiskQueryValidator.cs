using FluentValidation;

namespace DataAnalysis.Application.Features.ExecSummary.Queries.GetRisk;

public class GetRiskQueryValidator : AbstractValidator<GetRiskQuery>
{
    public GetRiskQueryValidator()
    {
        RuleFor(x => x.ProductGroup).IsInEnum();
        RuleFor(x => x.StartDate).NotEmpty();
        RuleFor(x => x.EndDate).NotEmpty().GreaterThanOrEqualTo(x => x.StartDate);
    }
}
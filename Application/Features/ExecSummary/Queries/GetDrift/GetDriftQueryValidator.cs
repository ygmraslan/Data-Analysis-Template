using FluentValidation;

namespace DataAnalysis.Application.Features.ExecSummary.Queries.GetDrift;

public class GetDriftQueryValidator : AbstractValidator<GetDriftQuery>
{
    public GetDriftQueryValidator()
    {
        RuleFor(x => x.ProductGroup).IsInEnum();
        RuleFor(x => x.StartDate).NotEmpty();
        RuleFor(x => x.EndDate).NotEmpty().GreaterThanOrEqualTo(x => x.StartDate);
    }
}
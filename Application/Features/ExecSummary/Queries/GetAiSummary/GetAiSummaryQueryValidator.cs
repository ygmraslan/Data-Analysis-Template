using FluentValidation;

namespace DataAnalysis.Application.Features.ExecSummary.Queries.GetAiSummary;

public class GetAiSummaryQueryValidator : AbstractValidator<GetAiSummaryQuery>
{
    public GetAiSummaryQueryValidator()
    {
        RuleFor(x => x.ProductGroup).IsInEnum();
        RuleFor(x => x.StartDate).NotEmpty();
        RuleFor(x => x.EndDate).NotEmpty().GreaterThanOrEqualTo(x => x.StartDate);
    }
}
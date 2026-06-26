using FluentValidation;

namespace DataAnalysis.Application.Features.Dashboard.Queries.GetKpi;

public class GetKpiQueryValidator : AbstractValidator<GetKpiQuery>
{
    public GetKpiQueryValidator()
    {
        RuleFor(x => x.ProductGroup)
            .IsInEnum()
            .WithMessage("Invalid product group. Must be KASKO or TRAFIK.");
    }
}
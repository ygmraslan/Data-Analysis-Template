using FluentValidation;

namespace DataAnalysis.Application.Features.Dashboard.Queries.GetHeatmap;

public class GetHeatmapQueryValidator : AbstractValidator<GetHeatmapQuery>
{
    public GetHeatmapQueryValidator()
    {
        RuleFor(x => x.ProductGroup)
            .IsInEnum()
            .WithMessage("Invalid product group. Must be KASKO or TRAFIK.");
    }
}
using FluentValidation;

namespace DataAnalysis.Application.Features.Dashboard.Queries.GetSegmentDrift;

public class GetSegmentDriftQueryValidator : AbstractValidator<GetSegmentDriftQuery>
{
    public GetSegmentDriftQueryValidator()
    {
        RuleFor(x => x.ProductGroup)
            .IsInEnum()
            .WithMessage("Invalid product group. Must be KASKO or TRAFIK.");
    }
}
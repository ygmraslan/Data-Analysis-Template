using FluentValidation;

namespace DataAnalysis.Application.Features.Dashboard.Queries.GetDistribution;

public class GetDistributionQueryValidator : AbstractValidator<GetDistributionQuery>
{
    public GetDistributionQueryValidator()
    {
        RuleFor(x => x.ProductGroup)
            .IsInEnum()
            .WithMessage("Invalid product group. Must be KASKO or TRAFIK.");

        RuleFor(x => x.DistributionType)
            .IsInEnum()
            .WithMessage("Invalid distribution type. Must be Brand, Region, VehicleAge or InsuredAge.");
    }
}
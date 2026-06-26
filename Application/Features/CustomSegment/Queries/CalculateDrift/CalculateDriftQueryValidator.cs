using FluentValidation;
using DataAnalysis.Application.Common.Enums;

namespace DataAnalysis.Application.Features.CustomSegment.Queries.CalculateDrift;

public class CalculateDriftQueryValidator : AbstractValidator<CalculateDriftQuery>
{
    public CalculateDriftQueryValidator()
    {
        RuleFor(x => x.ProductGroup)
            .NotEmpty()
            .Must(BeValidProductGroup).WithMessage("Geçersiz ürün grubu.");
        RuleFor(x => x.WeekStart).NotEmpty();
        RuleFor(x => x.WeekEnd).NotEmpty().GreaterThan(x => x.WeekStart);
        
        RuleFor(x => x)
            .Must(HasAtLeastOneFilter)
            .WithMessage("En az bir filtre seçilmelidir.");
    }

    private bool BeValidProductGroup(string productGroup)
    {
        return Enum.TryParse<ProductGroup>(productGroup, out _);
    }

    private bool HasAtLeastOneFilter(CalculateDriftQuery query)
    {
        return query.Filters.Brands?.Any() == true
            || query.Filters.InsuredAges?.Any() == true
            || query.Filters.InsuredTypes?.Any() == true
            || query.Filters.Genders?.Any() == true
            || query.Filters.VehicleAges?.Any() == true
            || query.Filters.VehicleValues?.Any() == true;
    }
}
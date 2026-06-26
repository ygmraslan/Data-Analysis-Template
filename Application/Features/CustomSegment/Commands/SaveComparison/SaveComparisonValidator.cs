using FluentValidation;
using DataAnalysis.Application.Common.Enums;
using DataAnalysis.Application.Features.CustomSegment.Common;

namespace DataAnalysis.Application.Features.CustomSegment.Commands.SaveComparison;

public class SaveComparisonValidator : AbstractValidator<SaveComparisonCommand>
{
    public SaveComparisonValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Karşılaştırma adı zorunludur.")
            .MaximumLength(100).WithMessage("Karşılaştırma adı en fazla 100 karakter olabilir.");

        RuleFor(x => x.ProductGroup)
            .NotEmpty().WithMessage("Ürün grubu zorunludur.")
            .Must(BeValidProductGroup).WithMessage("Geçersiz ürün grubu.");

        RuleFor(x => x.WeekStart)
            .NotEmpty().WithMessage("Başlangıç tarihi zorunludur.");

        RuleFor(x => x.WeekEnd)
            .NotEmpty().WithMessage("Bitiş tarihi zorunludur.")
            .GreaterThan(x => x.WeekStart).WithMessage("Bitiş tarihi başlangıç tarihinden sonra olmalı.");

        RuleFor(x => x.UserId).GreaterThan(0);
        
        RuleFor(x => x.SegmentA.Filters)
            .Must(HasAtLeastOneFilter)
            .WithMessage("Segment A için en az bir filtre seçilmelidir.");

        RuleFor(x => x.SegmentA.Filters)
            .Must(HasValidInsuredTypeAgeCombination)
            .WithMessage("Segment A: Tüzel müşteri seçildiğinde sigortalı yaşı yalnızca '0-17' olabilir.");

        RuleFor(x => x.SegmentB.Filters)
            .Must(HasAtLeastOneFilter)
            .WithMessage("Segment B için en az bir filtre seçilmelidir.");

        RuleFor(x => x.SegmentB.Filters)
            .Must(HasValidInsuredTypeAgeCombination)
            .WithMessage("Segment B: Tüzel müşteri seçildiğinde sigortalı yaşı yalnızca '0-17' olabilir.");

        RuleFor(x => x)
            .Must(HasDistinctFilters)
            .WithMessage("Segment A ve Segment B aynı filtrelere sahip olamaz.");
    }

    private static bool BeValidProductGroup(string productGroup)
    {
        return Enum.TryParse<ProductGroup>(productGroup, out _);
    }

    private static bool HasAtLeastOneFilter(SaveComparisonFilters filters)
    {
        return filters.Brands?.Any() == true
            || filters.InsuredAges?.Any() == true
            || filters.InsuredTypes?.Any() == true
            || filters.Genders?.Any() == true
            || filters.VehicleAges?.Any() == true
            || filters.VehicleValues?.Any() == true;
    }

    private static bool HasValidInsuredTypeAgeCombination(SaveComparisonFilters filters)
    {
        return InsuredTypeRule.IsAgeCombinationValid(filters.InsuredTypes, filters.InsuredAges);
    }

    private static bool HasDistinctFilters(SaveComparisonCommand cmd)
    {
        return !FiltersAreEqual(cmd.SegmentA.Filters, cmd.SegmentB.Filters);
    }

    private static bool FiltersAreEqual(SaveComparisonFilters a, SaveComparisonFilters b)
    {
        return ListEquals(a.Brands, b.Brands)
            && ListEquals(a.InsuredAges, b.InsuredAges)
            && ListEquals(a.InsuredTypes, b.InsuredTypes)
            && ListEquals(a.Genders, b.Genders)
            && ListEquals(a.VehicleAges, b.VehicleAges)
            && ListEquals(a.VehicleValues, b.VehicleValues);
    }

    private static bool ListEquals(List<string>? x, List<string>? y)
    {
        var setX = (x ?? new()).OrderBy(s => s);
        var setY = (y ?? new()).OrderBy(s => s);
        return setX.SequenceEqual(setY);
    }
}
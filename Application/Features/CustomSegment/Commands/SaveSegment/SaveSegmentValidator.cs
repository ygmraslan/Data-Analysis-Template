using FluentValidation;
using DataAnalysis.Application.Common.Enums;

namespace DataAnalysis.Application.Features.CustomSegment.Commands.SaveSegment;

public class SaveSegmentValidator : AbstractValidator<SaveSegmentCommand>
{
    public SaveSegmentValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Segment adı zorunludur.")
            .MaximumLength(100).WithMessage("Segment adı en fazla 100 karakter olabilir.");

        RuleFor(x => x.ProductGroup)
            .NotEmpty()
            .Must(BeValidProductGroup).WithMessage("Geçersiz ürün grubu.");
        RuleFor(x => x.WeekStart).NotEmpty();
        RuleFor(x => x.WeekEnd).NotEmpty().GreaterThan(x => x.WeekStart);
        RuleFor(x => x.UserId).GreaterThan(0);

        RuleFor(x => x)
            .Must(HasAtLeastOneFilter)
            .WithMessage("En az bir filtre seçilmelidir.");
    }

    private bool BeValidProductGroup(string productGroup)
    {
        return Enum.TryParse<ProductGroup>(productGroup, out _);
    }

    private bool HasAtLeastOneFilter(SaveSegmentCommand cmd)
    {
        return cmd.Filters.Brands?.Any() == true
            || cmd.Filters.InsuredAges?.Any() == true
            || cmd.Filters.InsuredTypes?.Any() == true
            || cmd.Filters.Genders?.Any() == true
            || cmd.Filters.VehicleAges?.Any() == true
            || cmd.Filters.VehicleValues?.Any() == true;
    }
}
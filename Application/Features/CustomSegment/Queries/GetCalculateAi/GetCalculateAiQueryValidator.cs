using FluentValidation;
using DataAnalysis.Application.Common.Enums;
using DataAnalysis.Application.Features.CustomSegment.Common;

namespace DataAnalysis.Application.Features.CustomSegment.Queries.GetCalculateAi;

public class GetCalculateAiQueryValidator : AbstractValidator<GetCalculateAiQuery>
{
    public GetCalculateAiQueryValidator()
    {
        RuleFor(x => x.ProductGroup)
            .NotEmpty().WithMessage("Ürün grubu gerekli.")
            .Must(BeValidProductGroup).WithMessage("Geçersiz ürün grubu.");

        RuleFor(x => x.ModelType)
            .IsInEnum().WithMessage("Geçersiz AI model tipi.");

        RuleFor(x => x.WeekStart)
            .NotEmpty().WithMessage("Başlangıç tarihi gerekli.");

        RuleFor(x => x.WeekEnd)
            .NotEmpty().WithMessage("Bitiş tarihi gerekli.")
            .GreaterThan(x => x.WeekStart).WithMessage("Bitiş tarihi başlangıçtan büyük olmalı.");

        RuleFor(x => x.Result)
            .NotNull().WithMessage("Sonuç verisi gerekli.");

        RuleFor(x => x)
            .Must(HasValidInsuredTypeAgeCombination)
            .WithMessage("Tüzel müşteri seçildiğinde sigortalı yaşı yalnızca '0-17' olabilir.");
    }

    private static bool BeValidProductGroup(string productGroup)
    {
        return Enum.TryParse<ProductGroup>(productGroup, true, out _);
    }

    private static bool HasValidInsuredTypeAgeCombination(GetCalculateAiQuery query)
    {
        if (query.Filters == null) return true;
        return InsuredTypeRule.IsAgeCombinationValid(query.Filters.InsuredTypes, query.Filters.InsuredAges);
    }
}
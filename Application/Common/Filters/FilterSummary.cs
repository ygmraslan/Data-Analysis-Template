using DataAnalysis.Application.Common.Enums;

namespace DataAnalysis.Application.Common.Filters;

public static class FilterSummary
{
    public static string Build(DetailFilter filter, ProductGroup group)
    {
        if (filter is null || filter.IsEmpty)
            return "Tümü";

        var parts = new List<string>();

        if (filter.InsuredTypes.Count > 0)
            parts.Add($"Sigortalı Türü: {string.Join(", ", filter.InsuredTypes.Select(v => v.ToLabel()))}");

        if (filter.BusinessSources.Count > 0)
            parts.Add($"İş Kaynağı: {string.Join(", ", filter.BusinessSources.Select(v => v.ToLabel()))}");

        if (filter.VehicleTypes.Count > 0)
            parts.Add($"Araç Tipi: {string.Join(", ", filter.VehicleTypes.Select(v => v.ToLabel()))}");

        if (filter.ProductCodes.Count > 0)
            parts.Add($"Ürün: {string.Join(", ", filter.ProductCodes)}");

        return string.Join(" · ", parts);
    }
}
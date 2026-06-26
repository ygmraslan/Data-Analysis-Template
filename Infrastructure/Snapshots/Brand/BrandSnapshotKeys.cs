using DataAnalysis.Application.Common.Enums;

namespace DataAnalysis.Infrastructure.Snapshots.Brand;

internal static class BrandSnapshotKeys
{
    public static string Kpi(ProductGroup pg) => $"brand:kpi:{pg}";
    public static string List(ProductGroup pg) => $"brand:list:{pg}";
    public static string Heatmap(ProductGroup pg) => $"brand:heatmap:{pg}";
    public static string Trend(ProductGroup pg, string brand) => $"brand:trend:{pg}:{brand}";
    public static string Models(ProductGroup pg, string brand) => $"brand:models:{pg}:{brand}";
}
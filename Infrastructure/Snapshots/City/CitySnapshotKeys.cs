using DataAnalysis.Application.Common.Enums;

namespace DataAnalysis.Infrastructure.Snapshots.City;

internal static class CitySnapshotKeys
{
    public static string Kpi(ProductGroup pg) => $"city:kpi:{pg}";
    public static string List(ProductGroup pg) => $"city:list:{pg}";
    public static string Heatmap(ProductGroup pg) => $"city:heatmap:{pg}";
    public static string Trend(ProductGroup pg, string city) => $"city:trend:{pg}:{city}";
    public static string TopBrands(ProductGroup pg, string city) => $"city:top-brands:{pg}:{city}";
    public static string Profile(ProductGroup pg, string city) => $"city:profile:{pg}:{city}";
}
using DataAnalysis.Application.Common.Enums;

namespace DataAnalysis.Infrastructure.Snapshots.Vehicle;

internal static class VehicleSnapshotKeys
{
    public static string Kpi(ProductGroup pg) => $"vehicle:kpi:{pg}";
    public static string Age(ProductGroup pg, bool grouped) => $"vehicle:age:{pg}:{grouped}";
    public static string Price(ProductGroup pg) => $"vehicle:price:{pg}";
    public static string Body(ProductGroup pg) => $"vehicle:body:{pg}";
    public static string Segment(ProductGroup pg) => $"vehicle:segment:{pg}";
    public static string AgeHeatmap(ProductGroup pg) => $"vehicle:age-heatmap:{pg}";
    public static string PriceHeatmap(ProductGroup pg) => $"vehicle:price-heatmap:{pg}";
    public static string AgeTrend(ProductGroup pg, string ageGroup, bool grouped) => $"vehicle:age-trend:{pg}:{ageGroup}:{grouped}";
    public static string PriceTrend(ProductGroup pg, string priceRange) => $"vehicle:price-trend:{pg}:{priceRange}";
}
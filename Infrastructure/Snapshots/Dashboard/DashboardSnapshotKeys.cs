using DataAnalysis.Application.Common.Enums;

namespace DataAnalysis.Infrastructure.Snapshots.Dashboard;

internal static class DashboardSnapshotKeys
{
    public static string Kpi(ProductGroup pg) => $"dashboard:kpi:{pg}";
    public static string SegmentDrift(ProductGroup pg) => $"dashboard:segment-drift:{pg}";
    public static string BrandDistribution(ProductGroup pg) => $"dashboard:brand-distribution:{pg}";
    public static string RegionDistribution(ProductGroup pg) => $"dashboard:region-distribution:{pg}";
    public static string VehicleAgeDistribution(ProductGroup pg) => $"dashboard:vehicle-age-distribution:{pg}";
    public static string InsuredAgeDistribution(ProductGroup pg) => $"dashboard:insured-age-distribution:{pg}";
    public static string Heatmap(ProductGroup pg) => $"dashboard:heatmap:{pg}";
    public static string WeeklyTotals(ProductGroup pg) => $"dashboard:weekly-totals:{pg}";
}
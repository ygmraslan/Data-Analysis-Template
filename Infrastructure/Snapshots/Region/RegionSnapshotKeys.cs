using DataAnalysis.Application.Common.Enums;

namespace DataAnalysis.Infrastructure.Snapshots.Region;

internal static class RegionSnapshotKeys
{
    public static string Kpi(ProductGroup pg) => $"region:kpi:{pg}";
    public static string Trend(ProductGroup pg) => $"region:trend:{pg}";
    public static string Heatmap(ProductGroup pg) => $"region:heatmap:{pg}";
}
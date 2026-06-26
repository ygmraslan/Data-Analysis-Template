using DataAnalysis.Application.Common.Enums;

namespace DataAnalysis.Infrastructure.Snapshots.Agency;

internal static class AgencySnapshotKeys
{
    public static string Kpi(ProductGroup pg) => $"agency:kpi:{pg}";
    public static string RegionDistribution(ProductGroup pg) => $"agency:region-distribution:{pg}";

    public static string Trend(ProductGroup pg, string agencyCode) => $"agency:trend:{pg}:{agencyCode}";
    public static string Profile(ProductGroup pg, string agencyCode) => $"agency:profile:{pg}:{agencyCode}";
    public static string TopBrands(ProductGroup pg, string agencyCode) => $"agency:top-brands:{pg}:{agencyCode}";

    public static string List(ProductGroup pg, int page, int pageSize, string? region)
    {
        var reg = region ?? "all";
        return $"agency:list:{pg}:{reg}:{page}:{pageSize}";
    }

    public static string TotalCount(ProductGroup pg, string? region)
    {
        var reg = region ?? "all";
        return $"agency:total-count:{pg}:{reg}";
    }

    public static string Heatmap(ProductGroup pg, int page, int pageSize) => $"agency:heatmap:{pg}:{page}:{pageSize}";
}
using DataAnalysis.Application.Common.Enums;

namespace DataAnalysis.Infrastructure.Snapshots.Company;

internal static class CompanySnapshotKeys
{
    public static string Kpi(ProductGroup pg) => $"company:kpi:{pg}";
    public static string List(ProductGroup pg) => $"company:list:{pg}";
    public static string Renewal(ProductGroup pg) => $"company:renewal:{pg}";
    public static string Heatmap(ProductGroup pg) => $"company:heatmap:{pg}";
    public static string Trend(ProductGroup pg, string company) => $"company:trend:{pg}:{company}";
    public static string TopBrands(ProductGroup pg, string company) => $"company:top-brands:{pg}:{company}";
    public static string Profile(ProductGroup pg, string company) => $"company:profile:{pg}:{company}";
    public static string StepDistribution(ProductGroup pg, string renewalType) => $"company:step-distribution:{pg}:{renewalType}";
}
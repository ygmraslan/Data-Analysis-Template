using DataAnalysis.Application.Common.Enums;

namespace DataAnalysis.Infrastructure.Snapshots.Demographic;

internal static class DemographicSnapshotKeys
{
    public static string Kpi(ProductGroup pg) => $"demographic:kpi:{pg}";
    public static string InsuredType(ProductGroup pg) => $"demographic:insured-type:{pg}";
    public static string Gender(ProductGroup pg) => $"demographic:gender:{pg}";
    public static string AgeGroup(ProductGroup pg) => $"demographic:age-group:{pg}";
    public static string InsuredCity(ProductGroup pg) => $"demographic:insured-city:{pg}";
}
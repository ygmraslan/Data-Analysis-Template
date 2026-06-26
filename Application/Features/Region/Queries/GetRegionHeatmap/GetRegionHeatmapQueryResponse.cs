namespace DataAnalysis.Application.Features.Region.Queries.GetRegionHeatmap;

public class GetRegionHeatmapQueryResponse
{
    public string Region { get; set; } = string.Empty;
    public string Week { get; set; } = string.Empty;
    public decimal AvgNetPremium { get; set; }
    public decimal PolicyRatio { get; set; }
}
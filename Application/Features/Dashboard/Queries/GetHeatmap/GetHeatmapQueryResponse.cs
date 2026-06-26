namespace DataAnalysis.Application.Features.Dashboard.Queries.GetHeatmap;

public class GetHeatmapQueryResponse
{
    public string Brand { get; set; } = string.Empty;
    public string Week { get; set; } = string.Empty;
    public decimal AvgNetPremium { get; set; }
    public decimal PolicyRatio { get; set; }
}
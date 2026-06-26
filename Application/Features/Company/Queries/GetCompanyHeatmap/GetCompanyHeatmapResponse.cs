namespace DataAnalysis.Application.Features.Company.Queries.GetCompanyHeatmap;

public class GetCompanyHeatmapResponse
{
    public string Company { get; set; } = string.Empty;
    public string Week { get; set; } = string.Empty;
    public decimal AvgNetPremium { get; set; }
    public decimal PolicyRatio { get; set; }
}
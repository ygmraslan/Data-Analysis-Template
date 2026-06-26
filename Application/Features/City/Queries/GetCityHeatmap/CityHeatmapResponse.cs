namespace DataAnalysis.Application.Features.City.Queries.GetCityHeatmap;

public class CityHeatmapResponse
{
    public string City { get; set; } = string.Empty;
    public string Week { get; set; } = string.Empty;
    public decimal AvgNetPremium { get; set; }
    public decimal PolicyRatio { get; set; }
}
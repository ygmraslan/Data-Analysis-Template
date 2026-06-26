namespace DataAnalysis.Application.Features.Region.Dtos;

public class RegionHeatmapDto
{
    public string Region { get; set; } = string.Empty;
    public string Week { get; set; } = string.Empty;
    public decimal AvgNetPremium { get; set; }
    public decimal PolicyRatio { get; set; }
}
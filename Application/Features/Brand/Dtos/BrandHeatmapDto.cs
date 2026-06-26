namespace DataAnalysis.Application.Features.Brand.Dtos;

public class BrandHeatmapDto
{
    public string Brand { get; set; } = string.Empty;
    public string Week { get; set; } = string.Empty;
    public decimal AvgNetPremium { get; set; }
    public decimal PolicyRatio { get; set; }
}
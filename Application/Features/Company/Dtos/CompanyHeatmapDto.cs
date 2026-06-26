namespace DataAnalysis.Application.Features.Company.Dtos;

public class CompanyHeatmapDto
{
    public string Company { get; set; } = string.Empty;
    public string Week { get; set; } = string.Empty;
    public decimal AvgNetPremium { get; set; }
    public decimal PolicyRatio { get; set; }
}
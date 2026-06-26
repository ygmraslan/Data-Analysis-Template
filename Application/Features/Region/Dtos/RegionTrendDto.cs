namespace DataAnalysis.Application.Features.Region.Dtos;

public class RegionTrendDto
{
    public string Region { get; set; } = string.Empty;
    public string WeekLabel { get; set; } = string.Empty;
    public decimal TotalPremium { get; set; }
    public decimal WoW { get; set; }
}
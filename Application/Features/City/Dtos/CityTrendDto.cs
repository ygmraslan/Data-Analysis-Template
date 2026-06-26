namespace DataAnalysis.Application.Features.City.Dtos;

public class CityTrendDto
{
    public string WeekLabel { get; set; } = string.Empty;
    public int PolicyCount { get; set; }
    public decimal NetPremium { get; set; }
    public decimal WoW { get; set; }
}
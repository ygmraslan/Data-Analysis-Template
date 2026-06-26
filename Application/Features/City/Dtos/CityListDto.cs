namespace DataAnalysis.Application.Features.City.Dtos;

public class CityListDto
{
    public string City { get; set; } = string.Empty;
    public int PolicyCount { get; set; }
    public decimal NetPremium { get; set; }
    public decimal AvgPremium { get; set; }
    public decimal WoW { get; set; }
}
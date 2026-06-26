namespace DataAnalysis.Application.Features.Brand.Dtos;

public class BrandModelDto
{
    public string Model { get; set; } = string.Empty;
    public int PolicyCount { get; set; }
    public decimal NetPremium { get; set; }
    public decimal AvgPremium { get; set; }
    public decimal WoW { get; set; }
}
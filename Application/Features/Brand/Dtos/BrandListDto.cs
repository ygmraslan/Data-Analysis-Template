namespace DataAnalysis.Application.Features.Brand.Dtos;

public class BrandListDto
{
    public string Brand { get; set; } = string.Empty;
    public int PolicyCount { get; set; }
    public decimal NetPremium { get; set; }
    public decimal WoW { get; set; }
}
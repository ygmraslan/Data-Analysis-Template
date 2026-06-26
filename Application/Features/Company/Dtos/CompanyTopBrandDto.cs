namespace DataAnalysis.Application.Features.Company.Dtos;

public class CompanyTopBrandDto
{
    public string Brand { get; set; } = string.Empty;
    public int PolicyCount { get; set; }
    public decimal NetPremium { get; set; }
}
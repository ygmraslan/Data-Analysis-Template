namespace DataAnalysis.Application.Features.Company.Dtos;

public class CompanyListDto
{
    public string Company { get; set; } = string.Empty;
    public int PolicyCount { get; set; }
    public decimal NetPremium { get; set; }
    public decimal AvgPremium { get; set; }
    public decimal WoW { get; set; }
}
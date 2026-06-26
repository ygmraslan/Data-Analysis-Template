namespace DataAnalysis.Application.Features.Company.Dtos;

public class CompanyTrendDto
{
    public string WeekLabel { get; set; } = string.Empty;
    public int PolicyCount { get; set; }
    public decimal NetPremium { get; set; }
    public decimal WoW { get; set; }
}
namespace DataAnalysis.Application.Features.Company.Dtos;

public class CompanyProfileDto
{
    public string Category { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public int PolicyCount { get; set; }
    public decimal NetPremium { get; set; }
}
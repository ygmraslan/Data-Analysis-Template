namespace DataAnalysis.Application.Features.Company.Dtos;

public class CompanyRenewalDto
{
    public string WeekLabel { get; set; } = string.Empty;
    
    public int NewBusinessCount { get; set; }
    public decimal NewBusinessPremium { get; set; }
    public decimal NewBusinessRatio { get; set; }
    
    public int TransferCount { get; set; }
    public decimal TransferPremium { get; set; }
    public decimal TransferRatio { get; set; }
    
    public int RenewalCount { get; set; }
    public decimal RenewalPremium { get; set; }
    public decimal RenewalRatio { get; set; }
}
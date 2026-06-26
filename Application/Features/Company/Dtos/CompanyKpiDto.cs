namespace DataAnalysis.Application.Features.Company.Dtos;

public class CompanyKpiDto
{
    public string TopCompanyByCount { get; set; } = string.Empty;
    public int TopCompanyCount { get; set; }
    public decimal TopCompanyCountWoW { get; set; }

    public string TopCompanyByPremium { get; set; } = string.Empty;
    public decimal TopCompanyPremium { get; set; }
    public decimal TopCompanyPremiumWoW { get; set; }

    public decimal NewBusinessRatio { get; set; }
    public decimal NewBusinessRatioWoW { get; set; }

    public decimal RenewalRatio { get; set; }
    public decimal RenewalRatioWoW { get; set; }

    public string PrevTopCompanyByCount { get; set; } = string.Empty;
    public int PrevTopCompanyCount { get; set; }

    public string PrevTopCompanyByPremium { get; set; } = string.Empty;
    public decimal PrevTopCompanyPremium { get; set; }

    public decimal PrevNewBusinessRatio { get; set; }
    public decimal PrevRenewalRatio { get; set; }

    public string DefaultCompany { get; set; } = string.Empty;
}
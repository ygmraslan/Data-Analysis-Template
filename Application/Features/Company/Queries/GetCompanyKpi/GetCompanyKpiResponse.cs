using System.Text.Json.Serialization;

namespace DataAnalysis.Application.Features.Company.Queries.GetCompanyKpi;

public class GetCompanyKpiResponse
{
    public string TopCompanyByCount { get; set; } = string.Empty;
    public int TopCompanyCount { get; set; }
    
    [JsonPropertyName("topCompanyCountWoW")]
    public decimal TopCompanyCountWoW { get; set; }

    public string TopCompanyByPremium { get; set; } = string.Empty;
    public decimal TopCompanyPremium { get; set; }
    
    [JsonPropertyName("topCompanyPremiumWoW")]
    public decimal TopCompanyPremiumWoW { get; set; }

    public decimal NewBusinessRatio { get; set; }
    
    [JsonPropertyName("newBusinessRatioWoW")]
    public decimal NewBusinessRatioWoW { get; set; }

    public decimal RenewalRatio { get; set; }
    
    [JsonPropertyName("renewalRatioWoW")]
    public decimal RenewalRatioWoW { get; set; }

    public string PrevTopCompanyByCount { get; set; } = string.Empty;
    public int PrevTopCompanyCount { get; set; }

    public string PrevTopCompanyByPremium { get; set; } = string.Empty;
    public decimal PrevTopCompanyPremium { get; set; }

    public decimal PrevNewBusinessRatio { get; set; }
    public decimal PrevRenewalRatio { get; set; }

    public string DefaultCompany { get; set; } = string.Empty;
}
using System.Text.Json.Serialization;

namespace DataAnalysis.Application.Features.Company.Queries.GetCompanyList;

public class GetCompanyListResponse
{
    public string Company { get; set; } = string.Empty;
    public int PolicyCount { get; set; }
    public decimal NetPremium { get; set; }
    public decimal AvgPremium { get; set; }
    
    [JsonPropertyName("wow")]
    public decimal WoW { get; set; }
}
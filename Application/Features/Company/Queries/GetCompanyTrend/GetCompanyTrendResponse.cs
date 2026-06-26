using System.Text.Json.Serialization;

namespace DataAnalysis.Application.Features.Company.Queries.GetCompanyTrend;

public class GetCompanyTrendResponse
{
    public string WeekLabel { get; set; } = string.Empty;
    public int PolicyCount { get; set; }
    public decimal NetPremium { get; set; }
    
    [JsonPropertyName("wow")]
    public decimal WoW { get; set; }
}
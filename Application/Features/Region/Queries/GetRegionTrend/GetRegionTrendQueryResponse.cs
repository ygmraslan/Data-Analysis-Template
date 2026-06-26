using System.Text.Json.Serialization;
namespace DataAnalysis.Application.Features.Region.Queries.GetRegionTrend;

public class GetRegionTrendQueryResponse
{
    public string Region { get; set; } = string.Empty;
    public string WeekLabel { get; set; } = string.Empty;
    public decimal TotalPremium { get; set; }
    [JsonPropertyName("wow")]

    public decimal WoW { get; set; }
}
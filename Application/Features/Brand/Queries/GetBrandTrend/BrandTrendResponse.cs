using System.Text.Json.Serialization;
namespace DataAnalysis.Application.Features.Brand.Queries.GetBrandTrend;

public class BrandTrendResponse
{
    public string WeekLabel { get; set; } = string.Empty;
    public int PolicyCount { get; set; }
    public decimal NetPremium { get; set; }
    [JsonPropertyName("wow")]

    public decimal WoW { get; set; }
}
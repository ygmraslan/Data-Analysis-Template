using System.Text.Json.Serialization;
namespace DataAnalysis.Application.Features.Brand.Queries.GetBrandList;

public class BrandListResponse
{
    public string Brand { get; set; } = string.Empty;
    public int PolicyCount { get; set; }
    public decimal NetPremium { get; set; }
    [JsonPropertyName("wow")]

    public decimal WoW { get; set; }
}
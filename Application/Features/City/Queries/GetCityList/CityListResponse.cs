using System.Text.Json.Serialization;

namespace DataAnalysis.Application.Features.City.Queries.GetCityList;

public class CityListResponse
{
    public string City { get; set; } = string.Empty;
    public int PolicyCount { get; set; }
    public decimal NetPremium { get; set; }
    public decimal AvgPremium { get; set; }
    [JsonPropertyName("wow")]
    public decimal WoW { get; set; }
}
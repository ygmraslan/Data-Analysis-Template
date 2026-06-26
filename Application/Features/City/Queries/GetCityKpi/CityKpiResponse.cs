using System.Text.Json.Serialization;

namespace DataAnalysis.Application.Features.City.Queries.GetCityKpi;

public class CityKpiResponse
{
    public string TopCity { get; set; } = string.Empty;
    public decimal TopCityPremium { get; set; }
    public string TopCityPrev { get; set; } = string.Empty;
    public decimal TopCityPrevPremium { get; set; }

    public string BottomCity { get; set; } = string.Empty;
    public decimal BottomCityPremium { get; set; }
    public string BottomCityPrev { get; set; } = string.Empty;
    public decimal BottomCityPrevPremium { get; set; }

    public string TopGainerCity { get; set; } = string.Empty;
    [JsonPropertyName("topGainerWoW")]
    public decimal TopGainerWoW { get; set; }
    public bool HasGainer { get; set; }
    public string PrevTopGainerCity { get; set; } = string.Empty;
    [JsonPropertyName("prevTopGainerWoW")]
    public decimal PrevTopGainerWoW { get; set; }

    public string TopLoserCity { get; set; } = string.Empty;
    [JsonPropertyName("topLoserWoW")]
    public decimal TopLoserWoW { get; set; }
    public bool HasLoser { get; set; }
    public string PrevTopLoserCity { get; set; } = string.Empty;
    [JsonPropertyName("prevTopLoserWoW")]
    public decimal PrevTopLoserWoW { get; set; }

    public string DefaultCity { get; set; } = string.Empty;
}
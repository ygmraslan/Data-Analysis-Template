using System.Text.Json.Serialization;

namespace DataAnalysis.Application.Features.Vehicle.Queries.GetVehicleKpi;

public class VehicleKpiResponse
{
    public string TopGainerAge { get; set; } = string.Empty;
    [JsonPropertyName("topGainerAgeWoW")]
    public decimal TopGainerAgeWoW { get; set; }
    public bool HasAgeGainer { get; set; }
    public string PrevTopGainerAge { get; set; } = string.Empty;
    [JsonPropertyName("prevTopGainerAgeWoW")]
    public decimal PrevTopGainerAgeWoW { get; set; }

    public string TopLoserAge { get; set; } = string.Empty;
    [JsonPropertyName("topLoserAgeWoW")]
    public decimal TopLoserAgeWoW { get; set; }
    public bool HasAgeLoser { get; set; }
    public string PrevTopLoserAge { get; set; } = string.Empty;
    [JsonPropertyName("prevTopLoserAgeWoW")]
    public decimal PrevTopLoserAgeWoW { get; set; }

    public string TopGainerPrice { get; set; } = string.Empty;
    [JsonPropertyName("topGainerPriceWoW")]
    public decimal TopGainerPriceWoW { get; set; }
    public bool HasPriceGainer { get; set; }
    public string PrevTopGainerPrice { get; set; } = string.Empty;
    [JsonPropertyName("prevTopGainerPriceWoW")]
    public decimal PrevTopGainerPriceWoW { get; set; }

    public string TopLoserPrice { get; set; } = string.Empty;
    [JsonPropertyName("topLoserPriceWoW")]
    public decimal TopLoserPriceWoW { get; set; }
    public bool HasPriceLoser { get; set; }
    public string PrevTopLoserPrice { get; set; } = string.Empty;
    [JsonPropertyName("prevTopLoserPriceWoW")]
    public decimal PrevTopLoserPriceWoW { get; set; }

    public string DefaultAgeGroup { get; set; } = string.Empty;
    public string DefaultPriceRange { get; set; } = string.Empty;
}
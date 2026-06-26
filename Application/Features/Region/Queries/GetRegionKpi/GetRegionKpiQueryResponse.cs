using System.Text.Json.Serialization;
namespace DataAnalysis.Application.Features.Region.Queries.GetRegionKpi;

public class GetRegionKpiQueryResponse
{
    public string TopRegion { get; set; } = string.Empty;
    public decimal TopRegionPremium { get; set; }
    public string TopRegionPrev { get; set; } = string.Empty;
    public decimal TopRegionPrevPremium { get; set; }

    public string BottomRegion { get; set; } = string.Empty;
    public decimal BottomRegionPremium { get; set; }
    public string BottomRegionPrev { get; set; } = string.Empty;
    public decimal BottomRegionPrevPremium { get; set; }

    public string TopGainerRegion { get; set; } = string.Empty;
    [JsonPropertyName("topGainerWoW")]

    public decimal TopGainerWoW { get; set; }
    public bool HasGainer { get; set; }

    public string TopLoserRegion { get; set; } = string.Empty;
    [JsonPropertyName("topLoserWoW")]

    public decimal TopLoserWoW { get; set; }
    public bool HasLoser { get; set; }

    public string PrevTopGainerRegion { get; set; } = string.Empty;
    [JsonPropertyName("prevTopGainerWoW")]

    public decimal PrevTopGainerWoW { get; set; }
    public string PrevTopLoserRegion { get; set; } = string.Empty;
    [JsonPropertyName("prevTopLoserWoW")]

    public decimal PrevTopLoserWoW { get; set; }
}
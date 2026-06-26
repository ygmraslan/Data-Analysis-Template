using System.Text.Json.Serialization;
namespace DataAnalysis.Application.Features.Brand.Queries.GetBrandKpi;

public class BrandKpiResponse
{
    public string TopBrand { get; set; } = string.Empty;
    public int TopBrandCount { get; set; }
    public string TopBrandPrev { get; set; } = string.Empty;
    public int TopBrandPrevCount { get; set; }

    public string BottomBrand { get; set; } = string.Empty;
    public int BottomBrandCount { get; set; }
    public string BottomBrandPrev { get; set; } = string.Empty;
    public int BottomBrandPrevCount { get; set; }

    public string TopGainerBrand { get; set; } = string.Empty;
    [JsonPropertyName("topGainerWoW")]

    public decimal TopGainerWoW { get; set; }
    public bool HasGainer { get; set; }
    public string PrevTopGainerBrand { get; set; } = string.Empty;
    [JsonPropertyName("prevTopGainerWoW")]

    public decimal PrevTopGainerWoW { get; set; }

    public string TopLoserBrand { get; set; } = string.Empty;
    [JsonPropertyName("topLoserWoW")]

    public decimal TopLoserWoW { get; set; }
    public bool HasLoser { get; set; }
    public string PrevTopLoserBrand { get; set; } = string.Empty;
    [JsonPropertyName("prevTopLoserWoW")]

    public decimal PrevTopLoserWoW { get; set; }

    public string DefaultBrand { get; set; } = string.Empty;
}
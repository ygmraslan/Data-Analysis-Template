namespace DataAnalysis.Application.Features.Region.Dtos;

public class RegionKpiDto
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
    public decimal TopGainerWoW { get; set; }
    public bool HasGainer { get; set; }

    public string TopLoserRegion { get; set; } = string.Empty;
    public decimal TopLoserWoW { get; set; }
    public bool HasLoser { get; set; }

    public string PrevTopGainerRegion { get; set; } = string.Empty;
    public decimal PrevTopGainerWoW { get; set; }
    public string PrevTopLoserRegion { get; set; } = string.Empty;
    public decimal PrevTopLoserWoW { get; set; }
}
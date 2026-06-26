namespace DataAnalysis.Application.Features.Demographic.Queries.GetDemoKpi;

public sealed record GetDemoKpiResponse
{
    public int IndividualCount { get; init; }
    public decimal IndividualRatio { get; init; }
    public decimal IndividualWoW { get; init; }
    
    public int PrevIndividualCount { get; init; }
    public decimal PrevIndividualRatio { get; init; }
    public decimal PrevIndividualWoW { get; init; }
    
    public int CorporateCount { get; init; }
    public decimal CorporateRatio { get; init; }
    public decimal CorporateWoW { get; init; }
    
    public int PrevCorporateCount { get; init; }
    public decimal PrevCorporateRatio { get; init; }
    public decimal PrevCorporateWoW { get; init; }
    
    public string TopPlateCity { get; init; } = string.Empty;
    public decimal TopPlateCityRatio { get; init; }
    public decimal TopPlateCityWoW { get; init; }
    
    public string PrevTopPlateCity { get; init; } = string.Empty;
    public decimal PrevTopPlateCityRatio { get; init; }
    
    public string DominantAgeGroup { get; init; } = string.Empty;
    public decimal DominantAgeRatio { get; init; }
    public decimal DominantAgeWoW { get; init; }
    
    public string PrevDominantAgeGroup { get; init; } = string.Empty;
    public decimal PrevDominantAgeRatio { get; init; }
}
namespace DataAnalysis.Application.Features.Agency.Dtos;

public sealed record AgencyKpiDto
{
    public string TopPremiumAgency { get; init; } = string.Empty;
    public decimal TopPremiumAmount { get; init; }

    public string PrevTopPremiumAgency { get; init; } = string.Empty;
    public decimal PrevTopPremiumAmount { get; init; }

    public string TopAvgPremiumAgency { get; init; } = string.Empty;
    public decimal TopAvgPremiumAmount { get; init; }

    public string PrevTopAvgPremiumAgency { get; init; } = string.Empty;
    public decimal PrevTopAvgPremiumAmount { get; init; }

    public int ActiveAgencyCount { get; init; }
    public int PrevActiveAgencyCount { get; init; }
    public decimal ActiveAgencyCountWoW { get; init; }
    public decimal PrevActiveAgencyCountWoW { get; init; }

    public decimal AvgPremiumPerAgency { get; init; }
    public decimal PrevAvgPremiumPerAgency { get; init; }
    public decimal AvgPremiumPerAgencyWoW { get; init; }
    public decimal PrevAvgPremiumPerAgencyWoW { get; init; }

    public string DefaultAgencyCode { get; init; } = string.Empty;
}
namespace DataAnalysis.Application.Features.Agency.Queries.GetAgencyProfile;

public sealed record GetAgencyProfileResponse
{
    public List<AgencyProfileItem> AracYasi { get; init; } = [];
    public List<AgencyProfileItem> Basamak { get; init; } = [];
    public List<AgencyProfileItem> YenilemeTipi { get; init; } = [];
    public List<AgencyProfileItem> SigortaliTuru { get; init; } = [];
    public List<AgencyTopBrandItem> TopBrands { get; init; } = [];
}

public sealed record AgencyProfileItem
{
    public string Type { get; init; } = string.Empty;
    public int PolicyCount { get; init; }
    public decimal NetPremium { get; init; }
    public decimal Ratio { get; init; }
}

public sealed record AgencyTopBrandItem
{
    public string Brand { get; init; } = string.Empty;
    public int PolicyCount { get; init; }
    public decimal NetPremium { get; init; }
    public decimal Ratio { get; init; }
}
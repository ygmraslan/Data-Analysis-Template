namespace DataAnalysis.Application.Features.Agency.Queries.GetAgencyRegion;

public sealed record GetAgencyRegionResponse
{
    public List<AgencyRegionItem> Items { get; init; } = [];
}

public sealed record AgencyRegionItem
{
    public string Region { get; init; } = string.Empty;
    public int PolicyCount { get; init; }
    public decimal NetPremium { get; init; }
    public decimal Ratio { get; init; }
    public decimal WowChange { get; init; }
}
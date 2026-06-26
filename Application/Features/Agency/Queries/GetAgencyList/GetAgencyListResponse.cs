namespace DataAnalysis.Application.Features.Agency.Queries.GetAgencyList;

public sealed record GetAgencyListResponse
{
    public List<AgencyListItem> Items { get; init; } = [];
    public int TotalCount { get; init; }
    public int Page { get; init; }
    public int PageSize { get; init; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
}

public sealed record AgencyListItem
{
    public string AgencyCode { get; init; } = string.Empty;
    public string AgencyName { get; init; } = string.Empty;
    public string Region { get; init; } = string.Empty;
    public int PolicyCount { get; init; }
    public decimal NetPremium { get; init; }
    public decimal AvgPremium { get; init; }
    public decimal WowChange { get; init; }
}
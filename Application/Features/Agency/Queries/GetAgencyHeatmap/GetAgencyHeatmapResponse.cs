namespace DataAnalysis.Application.Features.Agency.Queries.GetAgencyHeatmap;

public sealed record GetAgencyHeatmapResponse
{
    public List<string> Weeks { get; init; } = [];
    public List<AgencyHeatmapRow> Rows { get; init; } = [];
    public int TotalCount { get; init; }
    public int Page { get; init; }
    public int PageSize { get; init; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
}

public sealed record AgencyHeatmapRow
{
    public string AgencyCode { get; init; } = string.Empty;
    public string AgencyName { get; init; } = string.Empty;
    public List<AgencyHeatmapCell> Cells { get; init; } = [];
}

public sealed record AgencyHeatmapCell
{
    public string Week { get; init; } = string.Empty;
    public int PolicyCount { get; init; }
    public decimal NetPremium { get; init; }
    public decimal AvgPremium { get; init; }
    public decimal PolicyRatio { get; init; }
}
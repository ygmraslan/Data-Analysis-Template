namespace DataAnalysis.Application.Features.Agency.Queries.GetAgencyTrend;

public sealed record GetAgencyTrendResponse
{
    public List<AgencyTrendItem> Items { get; init; } = [];
}

public sealed record AgencyTrendItem
{
    public string Week { get; init; } = string.Empty;
    public DateTime WeekStart { get; init; }
    public int PolicyCount { get; init; }
    public decimal NetPremium { get; init; }
    public decimal AvgPremium { get; init; }
}
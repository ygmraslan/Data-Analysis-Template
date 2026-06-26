namespace DataAnalysis.Application.Features.Agency.Dtos;

public sealed record AgencyHeatmapDto
{
    public string AgencyCode { get; init; } = string.Empty;
    public string AgencyName { get; init; } = string.Empty;
    public string Week { get; init; } = string.Empty;
    public DateTime WeekStart { get; init; }
    public int PolicyCount { get; init; }
    public decimal NetPremium { get; init; }
    public decimal AvgPremium { get; init; }
    public decimal PolicyRatio { get; init; }
}
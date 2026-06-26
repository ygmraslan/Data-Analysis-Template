namespace DataAnalysis.Application.Features.Agency.Dtos;

public sealed record AgencyRegionDto
{
    public string Region { get; init; } = string.Empty;
    public int PolicyCount { get; init; }
    public decimal NetPremium { get; init; }
    public decimal Ratio { get; init; }
    public decimal WowChange { get; init; }
}
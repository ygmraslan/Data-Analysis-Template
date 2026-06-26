namespace DataAnalysis.Application.Features.Agency.Dtos;

public sealed record AgencyTopBrandDto
{
    public string Brand { get; init; } = string.Empty;
    public int PolicyCount { get; init; }
    public decimal NetPremium { get; init; }
}
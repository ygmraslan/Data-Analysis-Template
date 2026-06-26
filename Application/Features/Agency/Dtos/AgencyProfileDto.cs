namespace DataAnalysis.Application.Features.Agency.Dtos;

public sealed record AgencyProfileDto
{
    public string Category { get; init; } = string.Empty;  // AracYasi, Basamak, YenilemeTipi, SigortaliTuru
    public string Type { get; init; } = string.Empty;
    public int PolicyCount { get; init; }
    public decimal NetPremium { get; init; }
}
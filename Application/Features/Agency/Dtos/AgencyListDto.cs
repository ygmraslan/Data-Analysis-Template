namespace DataAnalysis.Application.Features.Agency.Dtos;

public sealed record AgencyListDto
{
    public string AgencyCode { get; init; } = string.Empty;
    public string AgencyName { get; init; } = string.Empty;
    public string Region { get; init; } = string.Empty;
    public int PolicyCount { get; init; }
    public decimal NetPremium { get; init; }
    public decimal AvgPremium { get; init; }
    public decimal WowChange { get; init; }  
}
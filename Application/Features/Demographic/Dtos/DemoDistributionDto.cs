namespace DataAnalysis.Application.Features.Demographic.Dtos;

public sealed record DemoDistributionDto
{
    public string Label { get; init; } = string.Empty;
    public int PolicyCount { get; init; }
    public decimal NetPremium { get; init; }
    public decimal AvgPremium { get; init; }
    public decimal Ratio { get; init; }
    public decimal WoW { get; init; }
}
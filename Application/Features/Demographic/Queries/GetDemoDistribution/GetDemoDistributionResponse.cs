namespace DataAnalysis.Application.Features.Demographic.Queries.GetDemoDistribution;

public class GetDemoDistributionResponse
{
    public string Label { get; set; } = string.Empty;
    public int PolicyCount { get; set; }
    public decimal NetPremium { get; set; }
    public decimal AvgPremium { get; set; }
    public decimal Ratio { get; set; }
    public decimal WoW { get; set; }
}
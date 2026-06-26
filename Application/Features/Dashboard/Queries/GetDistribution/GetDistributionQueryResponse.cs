namespace DataAnalysis.Application.Features.Dashboard.Queries.GetDistribution;

public class GetDistributionQueryResponse
{
    public string Label { get; set; } = string.Empty;
    public int Count { get; set; }
    public decimal Share { get; set; }
}
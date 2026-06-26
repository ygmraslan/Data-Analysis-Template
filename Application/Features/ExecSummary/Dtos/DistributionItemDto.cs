namespace DataAnalysis.Application.Features.ExecSummary.Dtos;

public class DistributionItemDto
{
    public string Label { get; set; } = string.Empty;
    public int Count { get; set; }
    public decimal Share { get; set; }
}
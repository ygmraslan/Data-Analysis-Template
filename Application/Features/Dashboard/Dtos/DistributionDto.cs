namespace DataAnalysis.Application.Features.Dashboard.Dtos;

public class DistributionDto
{
    public string Label { get; set; } = string.Empty;
    public int Count { get; set; }
    public decimal Share { get; set; }
}
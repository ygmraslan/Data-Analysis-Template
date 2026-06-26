namespace DataAnalysis.Application.Features.ExecSummary.Dtos;

public class RiskSegmentDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int PolicyCount { get; set; }
    public string Severity { get; set; } = string.Empty;
}
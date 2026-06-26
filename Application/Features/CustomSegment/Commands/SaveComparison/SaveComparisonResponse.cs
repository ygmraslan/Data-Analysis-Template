namespace DataAnalysis.Application.Features.CustomSegment.Commands.SaveComparison;

public class SaveComparisonResponse
{
    public int ComparisonId { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool Success { get; set; }
    public string? Message { get; set; }
    public int AiCommentsGenerated { get; set; }
}
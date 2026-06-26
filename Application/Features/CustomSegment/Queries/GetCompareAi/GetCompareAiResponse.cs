namespace DataAnalysis.Application.Features.CustomSegment.Queries.GetCompareAi;

public class GetCompareAiResponse
{
    public string Comment { get; set; } = string.Empty;
    public string ModelName { get; set; } = string.Empty;
    public bool Success { get; set; }
    public string? Error { get; set; }
}
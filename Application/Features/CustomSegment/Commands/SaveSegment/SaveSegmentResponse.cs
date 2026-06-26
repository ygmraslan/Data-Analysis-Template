namespace DataAnalysis.Application.Features.CustomSegment.Commands.SaveSegment;

public class SaveSegmentResponse
{
    public int SegmentId { get; set; }
    public int ResultId { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool Success { get; set; }
    public string? Message { get; set; }
}
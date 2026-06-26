namespace DataAnalysis.Application.Features.CustomSegment.Dtos;

public class SegmentCompareResultDto
{
    public int TotalPolicy { get; set; }
    
    // Segment 1
    public string Segment1Name { get; set; } = string.Empty;
    public int Segment1Count { get; set; }
    public decimal Segment1StartShare { get; set; }
    public decimal Segment1EndShare { get; set; }
    public decimal Segment1Change { get; set; }
    public decimal Segment1Growth { get; set; }
    
    // Segment 2
    public string Segment2Name { get; set; } = string.Empty;
    public int Segment2Count { get; set; }
    public decimal Segment2StartShare { get; set; }
    public decimal Segment2EndShare { get; set; }
    public decimal Segment2Change { get; set; }
    public decimal Segment2Growth { get; set; }
    
    public List<SegmentCompareWeekDto> WeeklyData { get; set; } = new();
    public string? AiComment { get; set; }
}
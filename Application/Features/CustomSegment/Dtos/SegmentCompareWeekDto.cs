namespace DataAnalysis.Application.Features.CustomSegment.Dtos;

public class SegmentCompareWeekDto
{
    public DateTime WeekStart { get; set; }
    public string WeekLabel { get; set; } = string.Empty;
    public int TotalPolicy { get; set; }
    public int Segment1Count { get; set; }
    public decimal Segment1Share { get; set; }
    public int Segment2Count { get; set; }
    public decimal Segment2Share { get; set; }
}
namespace DataAnalysis.Application.Features.CustomSegment.Dtos;

public class SegmentDriftWeekDto
{
    public DateTime WeekStart { get; set; }
    public string WeekLabel { get; set; } = string.Empty;
    public int TotalPolicy { get; set; }
    public int SegmentCount { get; set; }
    public decimal SegmentShare { get; set; }
}
namespace DataAnalysis.Application.Features.Dashboard.Dtos;

public class SegmentDriftDto
{
    public DateTime WeekStart { get; set; }
    public int TotalPolicy { get; set; }
    public decimal Seg1Share { get; set; }
    public decimal Seg2Share { get; set; }
    public decimal Seg1WoW { get; set; }
    public decimal Seg2WoW { get; set; }
    public decimal Seg1Rolling4 { get; set; }
    public decimal Seg2Rolling4 { get; set; }
}
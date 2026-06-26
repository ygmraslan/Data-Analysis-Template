namespace DataAnalysis.Application.Features.ExecSummary.Dtos;

public class DriftWeekDto
{
    public DateTime WeekStart { get; set; }
    public string WeekLabel { get; set; } = string.Empty;
    public int TotalPolicy { get; set; }
    public int Seg1Count { get; set; }
    public decimal Seg1Share { get; set; }
    public int Seg2Count { get; set; }
    public decimal Seg2Share { get; set; }
}
namespace DataAnalysis.Application.Features.CustomSegment.Dtos;

public class SegmentDriftResultDto
{
    public int TotalPolicy { get; set; }
    public int SegmentCount { get; set; }
    public decimal StartShare { get; set; }
    public decimal EndShare { get; set; }
    public decimal Change { get; set; }
    public decimal GrowthMultiple { get; set; }
    public List<SegmentDriftWeekDto> WeeklyData { get; set; } = new();
    public string? AiComment { get; set; }
}
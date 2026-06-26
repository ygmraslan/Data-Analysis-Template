namespace DataAnalysis.Application.Features.CustomSegment.Commands.RunSegment;

public class RunSegmentResponse
{
    public int ResultId { get; set; }
    public int TotalPolicy { get; set; }
    public int SegmentCount { get; set; }
    public decimal StartShare { get; set; }
    public decimal EndShare { get; set; }
    public decimal Change { get; set; }
    public decimal GrowthMultiple { get; set; }
    public List<RunSegmentWeekItem> WeeklyData { get; set; } = new();
    public string? AiCommentDeepSeek { get; set; }
    public string? AiCommentGemini { get; set; }
    public string? AiCommentGpt { get; set; }
    public bool FromCache { get; set; }
    public bool Success { get; set; }
    public string? Message { get; set; }
}

public class RunSegmentWeekItem
{
    public DateTime WeekStart { get; set; }
    public string WeekLabel { get; set; } = string.Empty;
    public int TotalPolicy { get; set; }
    public int SegmentCount { get; set; }
    public decimal SegmentShare { get; set; }
    public decimal? WoW { get; set; }
}
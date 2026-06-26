namespace DataAnalysis.Application.Features.CustomSegment.Dtos;

public class SegmentResultDto
{
    public int Id { get; set; }
    public int SegmentId { get; set; }
    
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    
    public int TotalPolicy { get; set; }
    public int SegmentCount { get; set; }
    public decimal StartShare { get; set; }
    public decimal EndShare { get; set; }
    public decimal Change { get; set; }
    public decimal GrowthMultiple { get; set; }
    
    public List<SegmentDriftWeekDto> WeeklyData { get; set; } = new();
    public string? AiCommentDeepSeek { get; set; }
    public string? AiCommentGemini { get; set; }
    public string? AiCommentGpt { get; set; }
    
    public DateTime CreatedDate { get; set; }
    public string? CreatedByName { get; set; }
}
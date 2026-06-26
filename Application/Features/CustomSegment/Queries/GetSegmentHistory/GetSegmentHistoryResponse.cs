namespace DataAnalysis.Application.Features.CustomSegment.Queries.GetSegmentHistory;

public class GetSegmentHistoryResponse
{
    public List<GetSegmentHistoryItem> Items { get; set; } = new();
}

public class GetSegmentHistoryItem
{
    public int Id { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int TotalPolicy { get; set; }
    public int SegmentCount { get; set; }
    public decimal StartShare { get; set; }
    public decimal EndShare { get; set; }
    public decimal Change { get; set; }
    public decimal GrowthMultiple { get; set; }
    public bool HasAiCommentDeepSeek { get; set; }
    public bool HasAiCommentGemini { get; set; }
    public bool HasAiCommentGpt { get; set; }
    public DateTime CreatedDate { get; set; }
}
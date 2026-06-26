using DataAnalysis.Domain.Common;
using DataAnalysis.Domain.Entities.Identity;

namespace DataAnalysis.Domain.Entities.CustomSegment;

public class CustomSegmentResult : BaseEntity
{
    public int SegmentId { get; set; }

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public int TotalPolicy { get; set; }
    public int SegmentCount { get; set; }
    public decimal StartShare { get; set; }
    public decimal EndShare { get; set; }
    public decimal Change { get; set; }
    public decimal GrowthMultiple { get; set; }

    public string WeeklyData { get; set; } = string.Empty;

    public string? AiCommentDeepSeek { get; set; }
    public string? AiCommentGemini { get; set; }
    public string? AiCommentGpt { get; set; }

    // Navigation
    public CustomSegment Segment { get; set; } = null!;
    public User? CreatedByUser { get; set; }
    public User? UpdatedByUser { get; set; }
    public User? DeletedByUser { get; set; }
}
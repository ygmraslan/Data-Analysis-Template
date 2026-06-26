using DataAnalysis.Domain.Common;
using DataAnalysis.Domain.Entities.Identity;

namespace DataAnalysis.Domain.Entities.CustomSegment;

public class ComparisonSegment : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string ProductGroup { get; set; } = string.Empty;

    public DateTime WeekStart { get; set; }
    public DateTime WeekEnd { get; set; }
    public string? AiCommentDeepSeek { get; set; }
    public string? AiCommentGemini { get; set; }
    public string? AiCommentGpt { get; set; }

    public ICollection<ComparisonSegmentResult> Results { get; set; } = new List<ComparisonSegmentResult>();

    public User? CreatedByUser { get; set; }
    public User? UpdatedByUser { get; set; }
    public User? DeletedByUser { get; set; }
}
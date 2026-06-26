using DataAnalysis.Domain.Entities.Identity;

namespace DataAnalysis.Domain.Entities.ExecSummary;
public class ExecAiCache
{
    public int Id { get; set; }
    public DateTime WeekStart { get; set; }
    public DateTime WeekEnd { get; set; }
    public string ProductType { get; set; } = string.Empty;
    public string ModelType { get; set; } = string.Empty; 
    public string SummaryJson { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int? CreatedByUserId { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int? UpdatedByUserId { get; set; }

    public User? CreatedByUser { get; set; }
    public User? UpdatedByUser { get; set; }
}
namespace DataAnalysis.Application.Features.ExecSummary.Dtos;

public class ExecAiCacheDto
{
    public int Id { get; set; }
    public DateTime WeekStart { get; set; }
    public DateTime WeekEnd { get; set; }
    public string ProductType { get; set; } = string.Empty;
    public string ModelType { get; set; } = string.Empty;
    public string SummaryJson { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
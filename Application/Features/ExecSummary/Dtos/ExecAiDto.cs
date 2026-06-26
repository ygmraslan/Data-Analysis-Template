namespace DataAnalysis.Application.Features.ExecSummary.Dtos;

public class ExecAiDto
{
    public GeneralStatusDto GeneralStatus { get; set; } = new();
    public PortfolioSummaryDto PortfolioSummary { get; set; } = new();
    public List<FindingDto> Findings { get; set; } = new();
    public List<RecommendationDto> Recommendations { get; set; } = new();
    public int CriticalCount { get; set; }
    public int HighCount { get; set; }
    public int MediumCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class PortfolioSummaryDto
{
    public List<string> Characteristics { get; set; } = new();
    public List<string> RiskAreas { get; set; } = new();
    public List<string> PositiveFactors { get; set; } = new();
}

public class GeneralStatusDto
{
    public string Summary { get; set; } = string.Empty;
    public List<MetricDto> Metrics { get; set; } = new();
}

public class MetricDto
{
    public string Label { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string Level { get; set; } = "info"; 
}
public class FindingDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Level { get; set; } = "medium"; 
}
public class RecommendationDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Icon { get; set; } = "⚡";
}
public class MultiModelExecAiDto
{
    public ModelResponseDto DeepSeek { get; set; } = new();
    public ModelResponseDto Gemini { get; set; } = new();
    public ModelResponseDto Gpt { get; set; } = new();
    public bool FromCache { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
 
public class ModelResponseDto
{
    public string ModelName { get; set; } = string.Empty;
    public string ModelDisplayName { get; set; } = string.Empty;
    public ExecAiDto? Data { get; set; }
    public bool Success { get; set; }
    public string? Error { get; set; }
    public int DurationMs { get; set; }
}
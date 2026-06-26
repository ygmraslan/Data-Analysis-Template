namespace DataAnalysis.Application.Features.CustomSegment.Dtos;

public class SaveComparisonRequestDto
{
    public string Name { get; set; } = string.Empty;
    public string ProductGroup { get; set; } = string.Empty;
    public DateTime WeekStart { get; set; }
    public DateTime WeekEnd { get; set; }

    public SegmentFilterDto SegmentAFilters { get; set; } = new();
    public ComparisonSideResultDto SegmentAResult { get; set; } = new();

    public SegmentFilterDto SegmentBFilters { get; set; } = new();
    public ComparisonSideResultDto SegmentBResult { get; set; } = new();

    public string? AiCommentDeepSeek { get; set; }
    public string? AiCommentGemini { get; set; }
    public string? AiCommentGpt { get; set; }
}

public class ComparisonSideResultDto
{
    public int TotalPolicy { get; set; }
    public int SegmentCount { get; set; }
    public decimal StartShare { get; set; }
    public decimal EndShare { get; set; }
    public decimal Change { get; set; }
    public decimal GrowthMultiple { get; set; }
    public List<SegmentDriftWeekDto> WeeklyData { get; set; } = new();
}
public class ComparisonSummaryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ProductGroup { get; set; } = string.Empty;
    public DateTime WeekStart { get; set; }
    public DateTime WeekEnd { get; set; }

    public DateTime CreatedDate { get; set; }
    public string? CreatedByName { get; set; }

    public ComparisonSideSummaryDto? SegmentA { get; set; }
    public ComparisonSideSummaryDto? SegmentB { get; set; }
}

public class ComparisonSideSummaryDto
{
    public SegmentFilterDto Filters { get; set; } = new();
    public decimal EndShare { get; set; }
    public decimal Change { get; set; }
}

public class ComparisonDetailDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ProductGroup { get; set; } = string.Empty;
    public DateTime WeekStart { get; set; }
    public DateTime WeekEnd { get; set; }

    public DateTime CreatedDate { get; set; }
    public string? CreatedByName { get; set; }

    public ComparisonSideDetailDto? SegmentA { get; set; }
    public ComparisonSideDetailDto? SegmentB { get; set; }

    public string? AiCommentDeepSeek { get; set; }
    public string? AiCommentGemini { get; set; }
    public string? AiCommentGpt { get; set; }
}

public class ComparisonSideDetailDto
{
    public SegmentFilterDto Filters { get; set; } = new();
    public int TotalPolicy { get; set; }
    public int SegmentCount { get; set; }
    public decimal StartShare { get; set; }
    public decimal EndShare { get; set; }
    public decimal Change { get; set; }
    public decimal GrowthMultiple { get; set; }
    public List<SegmentDriftWeekDto> WeeklyData { get; set; } = new();
}
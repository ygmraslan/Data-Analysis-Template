using DataAnalysis.Application.Features.CustomSegment.Dtos;

namespace DataAnalysis.Application.Features.CustomSegment.Queries.GetComparisonById;

public class GetComparisonByIdResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ProductGroup { get; set; } = string.Empty;
    public DateTime WeekStart { get; set; }
    public DateTime WeekEnd { get; set; }
    public DateTime CreatedDate { get; set; }
    public string? CreatedByName { get; set; }

    public GetComparisonByIdSide? SegmentA { get; set; }
    public GetComparisonByIdSide? SegmentB { get; set; }

    public string? AiCommentDeepSeek { get; set; }
    public string? AiCommentGemini { get; set; }
    public string? AiCommentGpt { get; set; }
}

public class GetComparisonByIdSide
{
    public GetComparisonByIdFilters Filters { get; set; } = new();

    public int TotalPolicy { get; set; }
    public int SegmentCount { get; set; }
    public decimal StartShare { get; set; }
    public decimal EndShare { get; set; }
    public decimal Change { get; set; }
    public decimal GrowthMultiple { get; set; }

    public List<SegmentDriftWeekDto> WeeklyData { get; set; } = new();
}

public class GetComparisonByIdFilters
{
    public List<string>? Brands { get; set; }
    public List<string>? InsuredAges { get; set; }
    public List<string>? InsuredTypes { get; set; }
    public List<string>? Genders { get; set; }
    public List<string>? VehicleAges { get; set; }
    public List<string>? VehicleValues { get; set; }
}
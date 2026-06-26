using DataAnalysis.Application.Features.CustomSegment.Dtos;

namespace DataAnalysis.Application.Features.CustomSegment.Queries.GetSegmentById;

public class GetSegmentByIdQueryResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ProductGroup { get; set; } = string.Empty;

    public GetSegmentByIdFilters Filters { get; set; } = new();

    public DateTime CreatedDate { get; set; }
    public string? CreatedByName { get; set; }

    public int ResultCount { get; set; }
    public GetSegmentByIdLastResult? LastResult { get; set; }
}

public class GetSegmentByIdFilters
{
    public List<string>? Brands { get; set; }
    public List<string>? InsuredAges { get; set; }
    public List<string>? InsuredTypes { get; set; }
    public List<string>? Genders { get; set; }
    public List<string>? VehicleAges { get; set; }
    public List<string>? VehicleValues { get; set; }
}

public class GetSegmentByIdLastResult
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
    public List<SegmentDriftWeekDto> WeeklyData { get; set; } = new();
    public string? AiCommentDeepSeek { get; set; }
    public string? AiCommentGemini { get; set; }
    public string? AiCommentGpt { get; set; }
    public DateTime CreatedDate { get; set; }
    public string? CreatedByName { get; set; }
}
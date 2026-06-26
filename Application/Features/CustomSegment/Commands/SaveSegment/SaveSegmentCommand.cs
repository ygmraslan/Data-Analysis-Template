using MediatR;
using DataAnalysis.Application.Features.CustomSegment.Dtos;

namespace DataAnalysis.Application.Features.CustomSegment.Commands.SaveSegment;

public class SaveSegmentCommand : IRequest<SaveSegmentResponse>
{
    public string Name { get; set; } = string.Empty;
    public string ProductGroup { get; set; } = string.Empty;
    public DateTime WeekStart { get; set; }
    public DateTime WeekEnd { get; set; }
    
    public SaveSegmentFilters Filters { get; set; } = new();

    public SaveSegmentDriftResult Result { get; set; } = new();

    public SaveSegmentAiComments? AiComments { get; set; }

    public int UserId { get; set; }
}

public class SaveSegmentFilters
{
    public List<string>? Brands { get; set; }
    public List<string>? InsuredAges { get; set; }
    public List<string>? InsuredTypes { get; set; }
    public List<string>? Genders { get; set; }
    public List<string>? VehicleAges { get; set; }
    public List<string>? VehicleValues { get; set; }
}

public class SaveSegmentDriftResult
{
    public int TotalPolicy { get; set; }
    public int SegmentCount { get; set; }
    public decimal StartShare { get; set; }
    public decimal EndShare { get; set; }
    public decimal Change { get; set; }
    public decimal GrowthMultiple { get; set; }
    public List<SegmentDriftWeekDto> WeeklyData { get; set; } = new();
}

public class SaveSegmentAiComments
{
    public string? DeepSeek { get; set; }
    public string? Gemini { get; set; }
    public string? Gpt { get; set; }
}
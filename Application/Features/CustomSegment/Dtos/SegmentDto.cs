namespace DataAnalysis.Application.Features.CustomSegment.Dtos;

public class SegmentDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ProductGroup { get; set; } = string.Empty;
    
    public SegmentFilterDto Filters { get; set; } = new();

    public string Description { get; set; } = string.Empty;

    public DateTime CreatedDate { get; set; }
    public string? CreatedByName { get; set; }

    public int ResultCount { get; set; }
    public SegmentResultDto? LastResult { get; set; }
}
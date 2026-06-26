namespace DataAnalysis.Application.Features.CustomSegment.Queries.GetSegments;

public class GetSegmentsQueryResponse
{
    public List<GetSegmentsItem> Items { get; set; } = new();
}

public class GetSegmentsItem
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ProductGroup { get; set; } = string.Empty;

    public GetSegmentsFilters Filters { get; set; } = new();

    public DateTime CreatedDate { get; set; }
    public string? CreatedByName { get; set; }
    
    public int ResultCount { get; set; }
    public GetSegmentsLastResult? LastResult { get; set; }
}

public class GetSegmentsFilters
{
    public List<string>? Brands { get; set; }
    public List<string>? InsuredAges { get; set; }
    public List<string>? InsuredTypes { get; set; }
    public List<string>? Genders { get; set; }
    public List<string>? VehicleAges { get; set; }
    public List<string>? VehicleValues { get; set; }
}

public class GetSegmentsLastResult
{
    public int Id { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal StartShare { get; set; }
    public decimal EndShare { get; set; }
    public decimal Change { get; set; }
    public decimal GrowthMultiple { get; set; }
    public DateTime CreatedDate { get; set; }
    public string? CreatedByName { get; set; }
}
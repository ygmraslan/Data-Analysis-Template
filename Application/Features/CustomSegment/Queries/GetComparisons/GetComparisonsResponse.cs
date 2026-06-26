namespace DataAnalysis.Application.Features.CustomSegment.Queries.GetComparisons;

public class GetComparisonsResponse
{
    public List<GetComparisonsItem> Items { get; set; } = new();
}

public class GetComparisonsItem
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ProductGroup { get; set; } = string.Empty;
    public DateTime WeekStart { get; set; }
    public DateTime WeekEnd { get; set; }
    public DateTime CreatedDate { get; set; }
    public string? CreatedByName { get; set; }

    public GetComparisonsSideItem? SegmentA { get; set; }
    public GetComparisonsSideItem? SegmentB { get; set; }
}

public class GetComparisonsSideItem
{
    public List<string>? Brands { get; set; }
    public List<string>? InsuredAges { get; set; }
    public List<string>? InsuredTypes { get; set; }
    public List<string>? Genders { get; set; }
    public List<string>? VehicleAges { get; set; }
    public List<string>? VehicleValues { get; set; }
    public decimal EndShare { get; set; }
    public decimal Change { get; set; }
}
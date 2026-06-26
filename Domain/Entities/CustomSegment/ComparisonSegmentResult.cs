using DataAnalysis.Domain.Common;
using DataAnalysis.Domain.Entities.Identity;

namespace DataAnalysis.Domain.Entities.CustomSegment;

public class ComparisonSegmentResult : BaseEntity
{
    public int ComparisonSegmentId { get; set; }
    public ComparisonSide Side { get; set; }
    public string? Brands { get; set; }
    public string? InsuredAges { get; set; }
    public string? InsuredTypes { get; set; }
    public string? Genders { get; set; }
    public string? VehicleAges { get; set; }
    public string? VehicleValues { get; set; }

    public int TotalPolicy { get; set; }
    public int SegmentCount { get; set; }
    public decimal StartShare { get; set; }
    public decimal EndShare { get; set; }
    public decimal Change { get; set; }
    public decimal GrowthMultiple { get; set; }
    public string WeeklyData { get; set; } = "[]";

    public ComparisonSegment Comparison { get; set; } = null!;
    public User? CreatedByUser { get; set; }
    public User? UpdatedByUser { get; set; }
    public User? DeletedByUser { get; set; }
}
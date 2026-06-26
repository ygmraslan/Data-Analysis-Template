using DataAnalysis.Domain.Common;
using DataAnalysis.Domain.Entities.Identity;

namespace DataAnalysis.Domain.Entities.CustomSegment;

public class CustomSegment : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string ProductGroup { get; set; } = string.Empty;    
    public string? Brands { get; set; }
    public string? InsuredAges { get; set; }
    public string? InsuredTypes { get; set; }
    public string? Genders { get; set; }
    public string? VehicleAges { get; set; }
    public string? VehicleValues { get; set; }
    
    // Navigation
    public User? CreatedByUser { get; set; }
    public User? UpdatedByUser { get; set; }
    public User? DeletedByUser { get; set; }
    public ICollection<CustomSegmentResult> Results { get; set; } = new List<CustomSegmentResult>();
}
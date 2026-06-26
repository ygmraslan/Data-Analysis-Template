namespace DataAnalysis.Application.Features.CustomSegment.Dtos;

public class SegmentOptionsDto
{
    public List<string> Brands { get; set; } = new();
    public List<string> InsuredAges { get; set; } = new();
    public List<string> InsuredTypes { get; set; } = new();
    public List<string> Genders { get; set; } = new();
    public List<string> VehicleAges { get; set; } = new();
    public List<string> VehicleValues { get; set; } = new();
}
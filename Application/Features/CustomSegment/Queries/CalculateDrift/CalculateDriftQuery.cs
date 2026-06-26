using MediatR;

namespace DataAnalysis.Application.Features.CustomSegment.Queries.CalculateDrift;

public class CalculateDriftQuery : IRequest<CalculateDriftQueryResponse>
{
    public string ProductGroup { get; set; } = string.Empty;
    public DateTime WeekStart { get; set; }
    public DateTime WeekEnd { get; set; }

    public CalculateDriftFilters Filters { get; set; } = new();
}

public class CalculateDriftFilters
{
    public List<string>? Brands { get; set; }
    public List<string>? InsuredAges { get; set; }
    public List<string>? InsuredTypes { get; set; }
    public List<string>? Genders { get; set; }
    public List<string>? VehicleAges { get; set; }
    public List<string>? VehicleValues { get; set; }
}
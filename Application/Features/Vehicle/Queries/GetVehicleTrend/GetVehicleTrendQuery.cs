using DataAnalysis.Application.Common.Enums;
using DataAnalysis.Application.Common.Filters;
using MediatR;

namespace DataAnalysis.Application.Features.Vehicle.Queries.GetVehicleTrend;

public enum VehicleTrendType { Age, Price }

public class GetVehicleTrendQuery : IRequest<List<VehicleTrendResponse>>, IFilteredQuery
{
    public ProductGroup ProductGroup { get; set; }
    public VehicleTrendType TrendType { get; set; } = VehicleTrendType.Age;
    public string Group { get; set; } = string.Empty;
    public bool Grouped { get; set; } = true;
    public DetailFilter Filter { get; set; } = new();
}
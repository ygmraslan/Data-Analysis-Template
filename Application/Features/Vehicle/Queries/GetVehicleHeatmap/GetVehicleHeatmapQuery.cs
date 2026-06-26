using DataAnalysis.Application.Common.Enums;
using DataAnalysis.Application.Common.Filters;
using MediatR;

namespace DataAnalysis.Application.Features.Vehicle.Queries.GetVehicleHeatmap;

public enum VehicleHeatmapType { Age, Price }

public class GetVehicleHeatmapQuery : IRequest<List<VehicleHeatmapResponse>>, IFilteredQuery
{
    public ProductGroup ProductGroup { get; set; }
    public VehicleHeatmapType HeatmapType { get; set; } = VehicleHeatmapType.Age;
    public DetailFilter Filter { get; set; } = new();
}
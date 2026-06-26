using DataAnalysis.Application.Common.Enums;
using DataAnalysis.Application.Common.Filters;
using MediatR;

namespace DataAnalysis.Application.Features.Vehicle.Queries.GetVehicleAge;

public class GetVehicleAgeQuery : IRequest<List<VehicleAgeResponse>>, IFilteredQuery
{
    public ProductGroup ProductGroup { get; set; }
    public bool Grouped { get; set; } = true;
    public DetailFilter Filter { get; set; } = new();
}
using DataAnalysis.Application.Common.Enums;
using DataAnalysis.Application.Common.Filters;
using MediatR;

namespace DataAnalysis.Application.Features.Vehicle.Queries.GetVehicleKpi;

public class GetVehicleKpiQuery : IRequest<VehicleKpiResponse>, IFilteredQuery
{
    public ProductGroup ProductGroup { get; set; }
    public DetailFilter Filter { get; set; } = new();
}
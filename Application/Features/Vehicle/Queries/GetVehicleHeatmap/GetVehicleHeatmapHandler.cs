using DataAnalysis.Application.Features.Vehicle.Abstractions;
using MediatR;

namespace DataAnalysis.Application.Features.Vehicle.Queries.GetVehicleHeatmap;

public class GetVehicleHeatmapHandler : IRequestHandler<GetVehicleHeatmapQuery, List<VehicleHeatmapResponse>>
{
    private readonly IVehicleRepository _repository;

    public GetVehicleHeatmapHandler(IVehicleRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<VehicleHeatmapResponse>> Handle(GetVehicleHeatmapQuery request, CancellationToken cancellationToken)
    {
        var list = request.HeatmapType == VehicleHeatmapType.Age
           ? await _repository.GetAgeHeatmapAsync(request.ProductGroup, request.Filter, cancellationToken)
            : await _repository.GetPriceHeatmapAsync(request.ProductGroup, request.Filter, cancellationToken);

        return list.Select(x => new VehicleHeatmapResponse
        {
            Label         = x.Label,
            Week          = x.Week,
            AvgNetPremium = x.AvgNetPremium,
            PolicyRatio   = x.PolicyRatio
        }).ToList();
    }
}
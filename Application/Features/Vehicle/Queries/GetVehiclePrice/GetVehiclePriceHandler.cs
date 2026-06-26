using DataAnalysis.Application.Features.Vehicle.Abstractions;
using MediatR;

namespace DataAnalysis.Application.Features.Vehicle.Queries.GetVehiclePrice;

public class GetVehiclePriceHandler : IRequestHandler<GetVehiclePriceQuery, List<VehiclePriceResponse>>
{
    private readonly IVehicleRepository _repository;

    public GetVehiclePriceHandler(IVehicleRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<VehiclePriceResponse>> Handle(GetVehiclePriceQuery request, CancellationToken cancellationToken)
    {
        var list = await _repository.GetPriceAsync(request.ProductGroup, request.Filter, cancellationToken);
        return list.Select(x => new VehiclePriceResponse
        {
            PriceRange  = x.PriceRange,
            PolicyCount = x.PolicyCount,
            NetPremium  = x.NetPremium,
            AvgPremium  = x.AvgPremium,
            WoW         = x.WoW,
        }).ToList();
    }
}
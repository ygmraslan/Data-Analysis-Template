using DataAnalysis.Application.Features.Vehicle.Abstractions;
using MediatR;

namespace DataAnalysis.Application.Features.Vehicle.Queries.GetVehicleAge;

public class GetVehicleAgeHandler : IRequestHandler<GetVehicleAgeQuery, List<VehicleAgeResponse>>
{
    private readonly IVehicleRepository _repository;

    public GetVehicleAgeHandler(IVehicleRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<VehicleAgeResponse>> Handle(GetVehicleAgeQuery request, CancellationToken cancellationToken)
    {
       var list = await _repository.GetAgeAsync(request.ProductGroup, request.Grouped, request.Filter, cancellationToken);
        return list.Select(x => new VehicleAgeResponse
        {
            AgeGroup    = x.AgeGroup,
            PolicyCount = x.PolicyCount,
            NetPremium  = x.NetPremium,
            AvgPremium  = x.AvgPremium,
            WoW         = x.WoW,
        }).ToList();
    }
}
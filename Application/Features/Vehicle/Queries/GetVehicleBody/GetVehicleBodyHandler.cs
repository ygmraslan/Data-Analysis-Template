using DataAnalysis.Application.Features.Vehicle.Abstractions;
using MediatR;

namespace DataAnalysis.Application.Features.Vehicle.Queries.GetVehicleBody;

public class GetVehicleBodyHandler : IRequestHandler<GetVehicleBodyQuery, List<VehicleBodyResponse>>
{
    private readonly IVehicleRepository _repository;

    public GetVehicleBodyHandler(IVehicleRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<VehicleBodyResponse>> Handle(GetVehicleBodyQuery request, CancellationToken cancellationToken)
    {
        var list = await _repository.GetBodyAsync(request.ProductGroup, request.Filter, cancellationToken);
        return list.Select(x => new VehicleBodyResponse
        {
            BodyType    = x.BodyType,
            PolicyCount = x.PolicyCount,
            NetPremium  = x.NetPremium,
            WoW         = x.WoW,
        }).ToList();
    }
}
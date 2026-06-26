using DataAnalysis.Application.Features.Vehicle.Abstractions;
using MediatR;

namespace DataAnalysis.Application.Features.Vehicle.Queries.GetVehicleTrend;

public class GetVehicleTrendHandler : IRequestHandler<GetVehicleTrendQuery, List<VehicleTrendResponse>>
{
    private readonly IVehicleRepository _repository;

    public GetVehicleTrendHandler(IVehicleRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<VehicleTrendResponse>> Handle(GetVehicleTrendQuery request, CancellationToken cancellationToken)
    {
        var list = request.TrendType == VehicleTrendType.Age
            ? await _repository.GetAgeTrendAsync(request.ProductGroup, request.Group, request.Grouped, request.Filter, cancellationToken)
            : await _repository.GetPriceTrendAsync(request.ProductGroup, request.Group, request.Filter, cancellationToken);

        return list.Select(x => new VehicleTrendResponse
        {
            WeekLabel   = x.WeekLabel,
            PolicyCount = x.PolicyCount,
            NetPremium  = x.NetPremium,
            WoW         = x.WoW,
        }).ToList();
    }
}
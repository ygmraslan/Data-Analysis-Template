using DataAnalysis.Application.Features.Vehicle.Abstractions;
using MediatR;

namespace DataAnalysis.Application.Features.Vehicle.Queries.GetVehicleSegment;

public class GetVehicleSegmentHandler : IRequestHandler<GetVehicleSegmentQuery, List<VehicleSegmentResponse>>
{
    private readonly IVehicleRepository _repository;

    public GetVehicleSegmentHandler(IVehicleRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<VehicleSegmentResponse>> Handle(GetVehicleSegmentQuery request, CancellationToken cancellationToken)
    {
        var list = await _repository.GetSegmentAsync(request.ProductGroup, request.Filter, cancellationToken);
        return list.Select(x => new VehicleSegmentResponse
        {
            Segment     = x.Segment,
            PolicyCount = x.PolicyCount,
            NetPremium  = x.NetPremium,
            WoW         = x.WoW,
        }).ToList();
    }
}
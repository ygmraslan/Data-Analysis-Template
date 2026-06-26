using DataAnalysis.Application.Features.Vehicle.Abstractions;
using MediatR;

namespace DataAnalysis.Application.Features.Vehicle.Queries.GetVehicleKpi;

public class GetVehicleKpiHandler : IRequestHandler<GetVehicleKpiQuery, VehicleKpiResponse>
{
    private readonly IVehicleRepository _repository;

    public GetVehicleKpiHandler(IVehicleRepository repository)
    {
        _repository = repository;
    }

    public async Task<VehicleKpiResponse> Handle(GetVehicleKpiQuery request, CancellationToken cancellationToken)
    {
        var dto = await _repository.GetKpiAsync(request.ProductGroup, request.Filter, cancellationToken);
        return new VehicleKpiResponse
        {
            TopGainerAge          = dto.TopGainerAge,
            TopGainerAgeWoW       = dto.TopGainerAgeWoW,
            HasAgeGainer          = dto.HasAgeGainer,
            PrevTopGainerAge      = dto.PrevTopGainerAge,
            PrevTopGainerAgeWoW   = dto.PrevTopGainerAgeWoW,
            TopLoserAge           = dto.TopLoserAge,
            TopLoserAgeWoW        = dto.TopLoserAgeWoW,
            HasAgeLoser           = dto.HasAgeLoser,
            PrevTopLoserAge       = dto.PrevTopLoserAge,
            PrevTopLoserAgeWoW    = dto.PrevTopLoserAgeWoW,
            TopGainerPrice        = dto.TopGainerPrice,
            TopGainerPriceWoW     = dto.TopGainerPriceWoW,
            HasPriceGainer        = dto.HasPriceGainer,
            PrevTopGainerPrice    = dto.PrevTopGainerPrice,
            PrevTopGainerPriceWoW = dto.PrevTopGainerPriceWoW,
            TopLoserPrice         = dto.TopLoserPrice,
            TopLoserPriceWoW      = dto.TopLoserPriceWoW,
            HasPriceLoser         = dto.HasPriceLoser,
            PrevTopLoserPrice     = dto.PrevTopLoserPrice,
            PrevTopLoserPriceWoW  = dto.PrevTopLoserPriceWoW,
            DefaultAgeGroup       = dto.DefaultAgeGroup,
            DefaultPriceRange     = dto.DefaultPriceRange,
        };
    }
}
using DataAnalysis.Application.Features.City.Abstractions;
using MediatR;

namespace DataAnalysis.Application.Features.City.Queries.GetCityKpi;

public class GetCityKpiHandler : IRequestHandler<GetCityKpiQuery, CityKpiResponse>
{
    private readonly ICityRepository _repository;

    public GetCityKpiHandler(ICityRepository repository)
    {
        _repository = repository;
    }

    public async Task<CityKpiResponse> Handle(GetCityKpiQuery request, CancellationToken cancellationToken)
    {
        var dto = await _repository.GetKpiAsync(request.ProductGroup, request.Filter, cancellationToken);
        return new CityKpiResponse
        {
            TopCity              = dto.TopCity,
            TopCityPremium       = dto.TopCityPremium,
            TopCityPrev          = dto.TopCityPrev,
            TopCityPrevPremium   = dto.TopCityPrevPremium,
            BottomCity           = dto.BottomCity,
            BottomCityPremium    = dto.BottomCityPremium,
            BottomCityPrev       = dto.BottomCityPrev,
            BottomCityPrevPremium = dto.BottomCityPrevPremium,
            TopGainerCity        = dto.TopGainerCity,
            TopGainerWoW         = dto.TopGainerWoW,
            HasGainer            = dto.HasGainer,
            PrevTopGainerCity    = dto.PrevTopGainerCity,
            PrevTopGainerWoW     = dto.PrevTopGainerWoW,
            TopLoserCity         = dto.TopLoserCity,
            TopLoserWoW          = dto.TopLoserWoW,
            HasLoser             = dto.HasLoser,
            PrevTopLoserCity     = dto.PrevTopLoserCity,
            PrevTopLoserWoW      = dto.PrevTopLoserWoW,
            DefaultCity          = dto.DefaultCity,
        };
    }
}
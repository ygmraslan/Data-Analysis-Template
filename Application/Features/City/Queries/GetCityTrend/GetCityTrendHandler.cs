using DataAnalysis.Application.Features.City.Abstractions;
using MediatR;

namespace DataAnalysis.Application.Features.City.Queries.GetCityTrend;

public class GetCityTrendHandler : IRequestHandler<GetCityTrendQuery, List<CityTrendResponse>>
{
    private readonly ICityRepository _repository;

    public GetCityTrendHandler(ICityRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<CityTrendResponse>> Handle(GetCityTrendQuery request, CancellationToken cancellationToken)
    {
        var list = await _repository.GetTrendAsync(request.ProductGroup, request.City, request.Filter, cancellationToken);
        return list.Select(x => new CityTrendResponse
        {
            WeekLabel   = x.WeekLabel,
            PolicyCount = x.PolicyCount,
            NetPremium  = x.NetPremium,
            WoW         = x.WoW,
        }).ToList();
    }
}
using DataAnalysis.Application.Features.City.Abstractions;
using MediatR;

namespace DataAnalysis.Application.Features.City.Queries.GetCityHeatmap;

public class GetCityHeatmapHandler : IRequestHandler<GetCityHeatmapQuery, List<CityHeatmapResponse>>
{
    private readonly ICityRepository _repository;

    public GetCityHeatmapHandler(ICityRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<CityHeatmapResponse>> Handle(GetCityHeatmapQuery request, CancellationToken cancellationToken)
    {
       var list = await _repository.GetHeatmapAsync(request.ProductGroup, request.Filter, cancellationToken);
        return list.Select(x => new CityHeatmapResponse
        {
            City          = x.City,
            Week          = x.Week,
            AvgNetPremium = x.AvgNetPremium,
            PolicyRatio   = x.PolicyRatio
        }).ToList();
    }
}
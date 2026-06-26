using DataAnalysis.Application.Features.City.Abstractions;
using MediatR;

namespace DataAnalysis.Application.Features.City.Queries.GetCityList;

public class GetCityListHandler : IRequestHandler<GetCityListQuery, List<CityListResponse>>
{
    private readonly ICityRepository _repository;

    public GetCityListHandler(ICityRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<CityListResponse>> Handle(GetCityListQuery request, CancellationToken cancellationToken)
    {
        var list = await _repository.GetListAsync(request.ProductGroup, request.Filter, cancellationToken);
        return list.Select(x => new CityListResponse
        {
            City        = x.City,
            PolicyCount = x.PolicyCount,
            NetPremium  = x.NetPremium,
            AvgPremium  = x.AvgPremium,
            WoW         = x.WoW,
        }).ToList();
    }
}
using DataAnalysis.Application.Features.City.Abstractions;
using MediatR;

namespace DataAnalysis.Application.Features.City.Queries.GetCityProfile;

public class GetCityProfileHandler : IRequestHandler<GetCityProfileQuery, CityProfileResponse>
{
    private readonly ICityRepository _repository;

    public GetCityProfileHandler(ICityRepository repository)
    {
        _repository = repository;
    }

    public async Task<CityProfileResponse> Handle(GetCityProfileQuery request, CancellationToken cancellationToken)
    {
        var brands  = await _repository.GetTopBrandsAsync(request.ProductGroup, request.City, request.Filter, cancellationToken);
        var profile = await _repository.GetProfileAsync(request.ProductGroup, request.City, request.Filter, cancellationToken);

        return new CityProfileResponse
        {
            TopBrands = brands.Select(x => new CityTopBrandResponse
            {
                Brand       = x.Brand,
                PolicyCount = x.PolicyCount,
                NetPremium  = x.NetPremium,
            }).ToList(),

            Profile = profile.Select(x => new CityProfileItemResponse
            {
                Category    = x.Category,
                Type        = x.Type,
                PolicyCount = x.PolicyCount,
                NetPremium  = x.NetPremium,
            }).ToList(),
        };
    }
}
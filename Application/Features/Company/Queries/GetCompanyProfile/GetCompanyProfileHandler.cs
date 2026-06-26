using DataAnalysis.Application.Features.Company.Abstractions;
using MediatR;

namespace DataAnalysis.Application.Features.Company.Queries.GetCompanyProfile;

public class GetCompanyProfileHandler : IRequestHandler<GetCompanyProfileQuery, GetCompanyProfileResponse>
{
    private readonly ICompanyRepository _repository;

    public GetCompanyProfileHandler(ICompanyRepository repository)
    {
        _repository = repository;
    }

    public async Task<GetCompanyProfileResponse> Handle(GetCompanyProfileQuery request, CancellationToken cancellationToken)
    {
       var topBrands = await _repository.GetTopBrandsAsync(request.ProductGroup, request.Company, request.Filter, cancellationToken);
        var profile = await _repository.GetProfileAsync(request.ProductGroup, request.Company, request.Filter, cancellationToken);

        return new GetCompanyProfileResponse
        {
            TopBrands = topBrands.Select(dto => new TopBrandItem
            {
                Brand = dto.Brand,
                PolicyCount = dto.PolicyCount,
                NetPremium = dto.NetPremium
            }).ToList(),
            Profile = profile.Select(dto => new ProfileItem
            {
                Category = dto.Category,
                Type = dto.Type,
                PolicyCount = dto.PolicyCount,
                NetPremium = dto.NetPremium
            }).ToList()
        };
    }
}
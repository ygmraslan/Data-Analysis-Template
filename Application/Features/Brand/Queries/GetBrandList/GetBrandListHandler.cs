using DataAnalysis.Application.Features.Brand.Abstractions;
using MediatR;

namespace DataAnalysis.Application.Features.Brand.Queries.GetBrandList;

public class GetBrandListHandler : IRequestHandler<GetBrandListQuery, List<BrandListResponse>>
{
    private readonly IBrandRepository _repository;

    public GetBrandListHandler(IBrandRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<BrandListResponse>> Handle(GetBrandListQuery request, CancellationToken cancellationToken)
    {
        var list = await _repository.GetListAsync(request.ProductGroup, request.Filter, cancellationToken);
        return list.Select(x => new BrandListResponse
        {
            Brand       = x.Brand,
            PolicyCount = x.PolicyCount,
            NetPremium  = x.NetPremium,
            WoW         = x.WoW,
        }).ToList();
    }
}
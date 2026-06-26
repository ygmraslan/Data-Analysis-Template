using DataAnalysis.Application.Features.Brand.Abstractions;
using MediatR;

namespace DataAnalysis.Application.Features.Brand.Queries.GetBrandModels;

public class GetBrandModelsHandler : IRequestHandler<GetBrandModelsQuery, List<BrandModelResponse>>
{
    private readonly IBrandRepository _repository;

    public GetBrandModelsHandler(IBrandRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<BrandModelResponse>> Handle(GetBrandModelsQuery request, CancellationToken cancellationToken)
    {
        var list = await _repository.GetModelsAsync(request.ProductGroup, request.Brand, request.Filter, cancellationToken);
        return list.Select(x => new BrandModelResponse
        {
            Model       = x.Model,
            PolicyCount = x.PolicyCount,
            NetPremium  = x.NetPremium,
            AvgPremium  = x.AvgPremium,
            WoW         = x.WoW,
        }).ToList();
    }
}
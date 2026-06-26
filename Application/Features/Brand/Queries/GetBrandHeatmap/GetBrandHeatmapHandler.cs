using DataAnalysis.Application.Features.Brand.Abstractions;
using MediatR;

namespace DataAnalysis.Application.Features.Brand.Queries.GetBrandHeatmap;

public class GetBrandHeatmapHandler : IRequestHandler<GetBrandHeatmapQuery, List<BrandHeatmapResponse>>
{
    private readonly IBrandRepository _repository;

    public GetBrandHeatmapHandler(IBrandRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<BrandHeatmapResponse>> Handle(GetBrandHeatmapQuery request, CancellationToken cancellationToken)
    {
        var list = await _repository.GetHeatmapAsync(request.ProductGroup, request.Filter, cancellationToken);
        return list.Select(x => new BrandHeatmapResponse
        {
            Brand         = x.Brand,
            Week          = x.Week,
            AvgNetPremium = x.AvgNetPremium,
            PolicyRatio   = x.PolicyRatio
        }).ToList();
    }
}
using DataAnalysis.Application.Features.Region.Abstractions;
using MediatR;

namespace DataAnalysis.Application.Features.Region.Queries.GetRegionHeatmap;

public class GetRegionHeatmapQueryHandler : IRequestHandler<GetRegionHeatmapQuery, List<GetRegionHeatmapQueryResponse>>
{
    private readonly IRegionRepository _regionRepository;

    public GetRegionHeatmapQueryHandler(IRegionRepository regionRepository)
    {
        _regionRepository = regionRepository;
    }

    public async Task<List<GetRegionHeatmapQueryResponse>> Handle(GetRegionHeatmapQuery request, CancellationToken cancellationToken)
    {
        var result = await _regionRepository.GetHeatmapAsync(request.ProductGroup, request.Filter, cancellationToken);

        return result.Select(x => new GetRegionHeatmapQueryResponse
        {
            Region        = x.Region,
            Week          = x.Week,
            AvgNetPremium = x.AvgNetPremium,
            PolicyRatio   = x.PolicyRatio
        }).ToList();
    }
}
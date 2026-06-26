using DataAnalysis.Application.Features.Region.Abstractions;
using MediatR;

namespace DataAnalysis.Application.Features.Region.Queries.GetRegionKpi;

public class GetRegionKpiQueryHandler : IRequestHandler<GetRegionKpiQuery, GetRegionKpiQueryResponse>
{
    private readonly IRegionRepository _regionRepository;

    public GetRegionKpiQueryHandler(IRegionRepository regionRepository)
    {
        _regionRepository = regionRepository;
    }

    public async Task<GetRegionKpiQueryResponse> Handle(GetRegionKpiQuery request, CancellationToken cancellationToken)
    {
        var result = await _regionRepository.GetKpiAsync(request.ProductGroup, request.Filter, cancellationToken);

        return new GetRegionKpiQueryResponse
        {
            TopRegion               = result.TopRegion,
            TopRegionPremium        = result.TopRegionPremium,
            TopRegionPrev           = result.TopRegionPrev,
            TopRegionPrevPremium    = result.TopRegionPrevPremium,
            BottomRegion            = result.BottomRegion,
            BottomRegionPremium     = result.BottomRegionPremium,
            BottomRegionPrev        = result.BottomRegionPrev,
            BottomRegionPrevPremium = result.BottomRegionPrevPremium,
            TopGainerRegion         = result.TopGainerRegion,
            TopGainerWoW            = result.TopGainerWoW,
            HasGainer               = result.HasGainer,
            TopLoserRegion          = result.TopLoserRegion,
            TopLoserWoW             = result.TopLoserWoW,
            HasLoser                = result.HasLoser,
            PrevTopGainerRegion     = result.PrevTopGainerRegion,
            PrevTopGainerWoW        = result.PrevTopGainerWoW,
            PrevTopLoserRegion      = result.PrevTopLoserRegion,
            PrevTopLoserWoW         = result.PrevTopLoserWoW
        };
    }
}
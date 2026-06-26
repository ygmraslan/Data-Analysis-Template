using DataAnalysis.Application.Features.Region.Abstractions;
using MediatR;

namespace DataAnalysis.Application.Features.Region.Queries.GetRegionTrend;

public class GetRegionTrendQueryHandler : IRequestHandler<GetRegionTrendQuery, List<GetRegionTrendQueryResponse>>
{
    private readonly IRegionRepository _regionRepository;

    public GetRegionTrendQueryHandler(IRegionRepository regionRepository)
    {
        _regionRepository = regionRepository;
    }

    public async Task<List<GetRegionTrendQueryResponse>> Handle(GetRegionTrendQuery request, CancellationToken cancellationToken)
    {
        var result = await _regionRepository.GetTrendAsync(request.ProductGroup, request.Filter, cancellationToken);

        return result.Select(x => new GetRegionTrendQueryResponse
        {
            Region       = x.Region,
            WeekLabel    = x.WeekLabel,
            TotalPremium = x.TotalPremium,
            WoW          = x.WoW
        }).ToList();
    }
}
using DataAnalysis.Application.Features.Dashboard.Abstractions;
using MediatR;

namespace DataAnalysis.Application.Features.Dashboard.Queries.GetHeatmap;

public class GetHeatmapQueryHandler : IRequestHandler<GetHeatmapQuery, List<GetHeatmapQueryResponse>>
{
    private readonly IDashboardRepository _dashboardRepository;

    public GetHeatmapQueryHandler(IDashboardRepository dashboardRepository)
    {
        _dashboardRepository = dashboardRepository;
    }

    public async Task<List<GetHeatmapQueryResponse>> Handle(GetHeatmapQuery request, CancellationToken cancellationToken)
    {
        var result = await _dashboardRepository.GetHeatmapAsync(request.ProductGroup, request.Filter, cancellationToken);

        return result.Select(x => new GetHeatmapQueryResponse
        {
            Brand = x.Brand,
            Week = x.Week,
            AvgNetPremium = x.AvgNetPremium,
            PolicyRatio = x.PolicyRatio
        }).ToList();
    }
}
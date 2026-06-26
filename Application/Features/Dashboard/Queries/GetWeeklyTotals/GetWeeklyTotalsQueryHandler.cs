using DataAnalysis.Application.Features.Dashboard.Abstractions;
using MediatR;

namespace DataAnalysis.Application.Features.Dashboard.Queries.GetWeeklyTotals;

public class GetWeeklyTotalsQueryHandler : IRequestHandler<GetWeeklyTotalsQuery, List<GetWeeklyTotalsQueryResponse>>
{
    private readonly IDashboardRepository _dashboardRepository;

    public GetWeeklyTotalsQueryHandler(IDashboardRepository dashboardRepository)
    {
        _dashboardRepository = dashboardRepository;
    }

    public async Task<List<GetWeeklyTotalsQueryResponse>> Handle(GetWeeklyTotalsQuery request, CancellationToken cancellationToken)
    {
        var data = await _dashboardRepository.GetWeeklyTotalsAsync(request.ProductGroup, request.Filter, cancellationToken);

        return data.Select(d => new GetWeeklyTotalsQueryResponse
        {
            WeekLabel     = d.WeekLabel,
            PolicyCount   = d.PolicyCount,
            NetPremium    = d.NetPremium,
            PolicyWoW     = d.PolicyWoW,
            NetPremiumWoW = d.NetPremiumWoW
        }).ToList();
    }
}
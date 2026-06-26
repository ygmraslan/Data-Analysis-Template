using DataAnalysis.Application.Features.Dashboard.Abstractions;
using MediatR;

namespace DataAnalysis.Application.Features.Dashboard.Queries.GetKpi;

public class GetKpiQueryHandler : IRequestHandler<GetKpiQuery, GetKpiQueryResponse>
{
    private readonly IDashboardRepository _dashboardRepository;

    public GetKpiQueryHandler(IDashboardRepository dashboardRepository)
    {
        _dashboardRepository = dashboardRepository;
    }

    public async Task<GetKpiQueryResponse> Handle(GetKpiQuery request, CancellationToken cancellationToken)
    {
        var result = await _dashboardRepository.GetKpiAsync(request.ProductGroup, request.Filter, cancellationToken);

        return new GetKpiQueryResponse
        {
            WeeklyPolicyCount    = result.WeeklyPolicyCount,
            WeeklyNetPremium     = result.WeeklyNetPremium,
            ZeroStepRatio        = result.ZeroStepRatio,
            PolicyWoW            = result.PolicyWoW,
            NetPremiumWoW        = result.NetPremiumWoW,
            ZeroStepWoW          = result.ZeroStepWoW,
            PrevWeeklyPolicyCount = result.PrevWeeklyPolicyCount,
            PrevWeeklyNetPremium  = result.PrevWeeklyNetPremium
        };
    }
}
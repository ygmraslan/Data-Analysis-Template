using DataAnalysis.Application.Features.Agency.Abstractions;
using MediatR;

namespace DataAnalysis.Application.Features.Agency.Queries.GetAgencyTrend;

public sealed class GetAgencyTrendHandler(IAgencyRepository repository)
    : IRequestHandler<GetAgencyTrendQuery, GetAgencyTrendResponse>
{
    public async Task<GetAgencyTrendResponse> Handle(GetAgencyTrendQuery request, CancellationToken cancellationToken)
    {
        var list = await repository.GetTrendAsync(request.ProductGroup, request.AgencyCode, request.Filter, cancellationToken);

        return new GetAgencyTrendResponse
        {
            Items = list.Select(x => new AgencyTrendItem
            {
                Week        = x.Week,
                WeekStart   = x.WeekStart,
                PolicyCount = x.PolicyCount,
                NetPremium  = x.NetPremium,
                AvgPremium  = x.AvgPremium
            }).ToList()
        };
    }
}
using DataAnalysis.Application.Features.Agency.Abstractions;
using MediatR;

namespace DataAnalysis.Application.Features.Agency.Queries.GetAgencyRegion;

public sealed class GetAgencyRegionHandler(IAgencyRepository repository)
    : IRequestHandler<GetAgencyRegionQuery, GetAgencyRegionResponse>
{
    public async Task<GetAgencyRegionResponse> Handle(GetAgencyRegionQuery request, CancellationToken cancellationToken)
    {
        var list = await repository.GetRegionDistributionAsync(request.ProductGroup, request.Filter, cancellationToken);

        return new GetAgencyRegionResponse
        {
            Items = list.Select(x => new AgencyRegionItem
            {
                Region      = x.Region,
                PolicyCount = x.PolicyCount,
                NetPremium  = x.NetPremium,
                Ratio       = x.Ratio,
                WowChange   = x.WowChange
            }).ToList()
        };
    }
}
using DataAnalysis.Application.Features.Agency.Abstractions;
using MediatR;

namespace DataAnalysis.Application.Features.Agency.Queries.GetAgencyList;

public sealed class GetAgencyListHandler(IAgencyRepository repository)
    : IRequestHandler<GetAgencyListQuery, GetAgencyListResponse>
{
    public async Task<GetAgencyListResponse> Handle(GetAgencyListQuery request, CancellationToken cancellationToken)
    {
        var listTask  = repository.GetListAsync(request.ProductGroup, request.Filter, request.Page, request.PageSize, request.Region, cancellationToken);
        var countTask = repository.GetTotalCountAsync(request.ProductGroup, request.Filter, request.Region, cancellationToken);

        await Task.WhenAll(listTask, countTask);

        var list  = listTask.Result;
        var count = countTask.Result;

        return new GetAgencyListResponse
        {
            Items = list.Select(x => new AgencyListItem
            {
                AgencyCode  = x.AgencyCode,
                AgencyName  = x.AgencyName,
                Region      = x.Region,
                PolicyCount = x.PolicyCount,
                NetPremium  = x.NetPremium,
                AvgPremium  = x.AvgPremium,
                WowChange   = x.WowChange
            }).ToList(),
            TotalCount = count,
            Page       = request.Page,
            PageSize   = request.PageSize
        };
    }
}
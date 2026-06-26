using DataAnalysis.Application.Features.Agency.Abstractions;
using MediatR;

namespace DataAnalysis.Application.Features.Agency.Queries.GetAgencyHeatmap;

public sealed class GetAgencyHeatmapHandler(IAgencyRepository repository)
    : IRequestHandler<GetAgencyHeatmapQuery, GetAgencyHeatmapResponse>
{
    public async Task<GetAgencyHeatmapResponse> Handle(GetAgencyHeatmapQuery request, CancellationToken cancellationToken)
    {
        var dataTask  = repository.GetHeatmapAsync(request.ProductGroup, request.Filter, request.Page, request.PageSize, cancellationToken);
        var countTask = repository.GetTotalCountAsync(request.ProductGroup, request.Filter, null, cancellationToken);

        await Task.WhenAll(dataTask, countTask);

        var data  = dataTask.Result;
        var count = countTask.Result;

        var weeks = data
            .Select(x => x.Week)
            .Distinct()
            .OrderBy(x => x)
            .ToList();

        var rows = data
            .GroupBy(x => new { x.AgencyCode, x.AgencyName })
            .Select(g => new AgencyHeatmapRow
            {
                AgencyCode = g.Key.AgencyCode,
                AgencyName = g.Key.AgencyName,
                Cells = weeks.Select(week =>
                {
                    var cell = g.FirstOrDefault(x => x.Week == week);
                    return new AgencyHeatmapCell
                    {
                        Week        = week,
                        PolicyCount = cell?.PolicyCount ?? 0,
                        NetPremium  = cell?.NetPremium ?? 0,
                        AvgPremium  = cell?.AvgPremium ?? 0,
                        PolicyRatio = cell?.PolicyRatio ?? 0

                    };
                }).ToList()
            })
            .ToList();

        return new GetAgencyHeatmapResponse
        {
            Weeks      = weeks,
            Rows       = rows,
            TotalCount = count,
            Page       = request.Page,
            PageSize   = request.PageSize
        };
    }
}
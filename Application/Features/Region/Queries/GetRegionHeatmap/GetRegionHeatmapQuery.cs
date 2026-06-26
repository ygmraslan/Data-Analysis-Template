using DataAnalysis.Application.Common.Enums;
using DataAnalysis.Application.Common.Filters;
using MediatR;

namespace DataAnalysis.Application.Features.Region.Queries.GetRegionHeatmap;

public class GetRegionHeatmapQuery : IRequest<List<GetRegionHeatmapQueryResponse>>, IFilteredQuery
{
    public ProductGroup ProductGroup { get; set; }
    public DetailFilter Filter { get; set; } = new();
}
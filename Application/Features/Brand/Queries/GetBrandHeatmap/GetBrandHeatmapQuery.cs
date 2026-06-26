using DataAnalysis.Application.Common.Enums;
using DataAnalysis.Application.Common.Filters;
using MediatR;

namespace DataAnalysis.Application.Features.Brand.Queries.GetBrandHeatmap;

public class GetBrandHeatmapQuery : IRequest<List<BrandHeatmapResponse>>, IFilteredQuery
{
    public ProductGroup ProductGroup { get; set; }
    public DetailFilter Filter { get; set; } = new();
}
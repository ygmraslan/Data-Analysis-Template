using DataAnalysis.Application.Common.Enums;
using DataAnalysis.Application.Common.Filters;
using MediatR;

namespace DataAnalysis.Application.Features.Company.Queries.GetCompanyHeatmap;

public record GetCompanyHeatmapQuery(ProductGroup ProductGroup) : IRequest<List<GetCompanyHeatmapResponse>>, IFilteredQuery
{
    public DetailFilter Filter { get; init; } = new();
}
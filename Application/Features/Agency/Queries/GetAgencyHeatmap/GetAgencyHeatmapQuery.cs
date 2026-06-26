using DataAnalysis.Application.Common.Enums;
using DataAnalysis.Application.Common.Filters;
using MediatR;

namespace DataAnalysis.Application.Features.Agency.Queries.GetAgencyHeatmap;

public sealed record GetAgencyHeatmapQuery(
    ProductGroup ProductGroup,
    int Page = 1,
    int PageSize = 10
) : IRequest<GetAgencyHeatmapResponse>, IFilteredQuery
{
    public DetailFilter Filter { get; init; } = new();
}
using DataAnalysis.Application.Common.Enums;
using DataAnalysis.Application.Common.Filters;
using MediatR;

namespace DataAnalysis.Application.Features.Agency.Queries.GetAgencyRegion;

public sealed record GetAgencyRegionQuery(ProductGroup ProductGroup) : IRequest<GetAgencyRegionResponse>, IFilteredQuery
{
    public DetailFilter Filter { get; init; } = new();
}
using DataAnalysis.Application.Common.Enums;
using DataAnalysis.Application.Common.Filters;
using MediatR;

namespace DataAnalysis.Application.Features.Agency.Queries.GetAgencyList;

public sealed record GetAgencyListQuery(
    ProductGroup ProductGroup,
    int Page = 1,
    int PageSize = 15,
    string? Region = null
) : IRequest<GetAgencyListResponse>, IFilteredQuery
{
    public DetailFilter Filter { get; init; } = new();
}
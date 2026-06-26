using DataAnalysis.Application.Common.Enums;
using DataAnalysis.Application.Common.Filters;
using MediatR;

namespace DataAnalysis.Application.Features.Agency.Queries.GetAgencyTrend;

public sealed record GetAgencyTrendQuery(
    ProductGroup ProductGroup,
    string AgencyCode
) : IRequest<GetAgencyTrendResponse>, IFilteredQuery
{
    public DetailFilter Filter { get; init; } = new();
}
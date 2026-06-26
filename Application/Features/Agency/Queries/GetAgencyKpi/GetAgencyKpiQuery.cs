using DataAnalysis.Application.Common.Enums;
using DataAnalysis.Application.Common.Filters;
using MediatR;

namespace DataAnalysis.Application.Features.Agency.Queries.GetAgencyKpi;

public sealed record GetAgencyKpiQuery(ProductGroup ProductGroup) : IRequest<GetAgencyKpiResponse>, IFilteredQuery
{
    public DetailFilter Filter { get; init; } = new();
}
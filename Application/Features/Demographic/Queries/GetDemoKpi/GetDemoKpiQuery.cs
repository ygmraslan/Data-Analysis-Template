using DataAnalysis.Application.Common.Enums;
using DataAnalysis.Application.Common.Filters;
using MediatR;

namespace DataAnalysis.Application.Features.Demographic.Queries.GetDemoKpi;

public sealed record GetDemoKpiQuery(ProductGroup ProductGroup) : IRequest<GetDemoKpiResponse>, IFilteredQuery
{
    public DetailFilter Filter { get; init; } = new();
}
using DataAnalysis.Application.Common.Enums;
using DataAnalysis.Application.Common.Filters;
using MediatR;

namespace DataAnalysis.Application.Features.Company.Queries.GetCompanyKpi;

public record GetCompanyKpiQuery(ProductGroup ProductGroup) : IRequest<GetCompanyKpiResponse>, IFilteredQuery
{
    public DetailFilter Filter { get; init; } = new();
}
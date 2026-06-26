using DataAnalysis.Application.Common.Enums;
using DataAnalysis.Application.Common.Filters;
using MediatR;

namespace DataAnalysis.Application.Features.Company.Queries.GetCompanyTrend;

public record GetCompanyTrendQuery(ProductGroup ProductGroup, string Company) : IRequest<List<GetCompanyTrendResponse>>, IFilteredQuery
{
    public DetailFilter Filter { get; init; } = new();
}
using DataAnalysis.Application.Common.Enums;
using DataAnalysis.Application.Common.Filters;
using MediatR;

namespace DataAnalysis.Application.Features.Company.Queries.GetCompanyList;

public record GetCompanyListQuery(ProductGroup ProductGroup) : IRequest<List<GetCompanyListResponse>>, IFilteredQuery
{
    public DetailFilter Filter { get; init; } = new();
}
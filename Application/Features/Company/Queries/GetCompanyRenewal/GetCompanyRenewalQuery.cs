using DataAnalysis.Application.Common.Enums;
using DataAnalysis.Application.Common.Filters;
using MediatR;

namespace DataAnalysis.Application.Features.Company.Queries.GetCompanyRenewal;

public record GetCompanyRenewalQuery(ProductGroup ProductGroup) : IRequest<List<GetCompanyRenewalResponse>>, IFilteredQuery
{
    public DetailFilter Filter { get; init; } = new();
}
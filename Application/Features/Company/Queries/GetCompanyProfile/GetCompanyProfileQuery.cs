using DataAnalysis.Application.Common.Enums;
using DataAnalysis.Application.Common.Filters;
using MediatR;

namespace DataAnalysis.Application.Features.Company.Queries.GetCompanyProfile;

public record GetCompanyProfileQuery(ProductGroup ProductGroup, string Company) : IRequest<GetCompanyProfileResponse>, IFilteredQuery
{
    public DetailFilter Filter { get; init; } = new();
}
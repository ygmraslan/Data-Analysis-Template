using DataAnalysis.Application.Common.Enums;
using DataAnalysis.Application.Common.Filters;
using MediatR;

namespace DataAnalysis.Application.Features.Agency.Queries.GetAgencyProfile;

public sealed record GetAgencyProfileQuery(
    ProductGroup ProductGroup,
    string AgencyCode
) : IRequest<GetAgencyProfileResponse>, IFilteredQuery
{
    public DetailFilter Filter { get; init; } = new();
}
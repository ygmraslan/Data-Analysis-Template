using DataAnalysis.Application.Common.Enums;
using DataAnalysis.Application.Common.Filters;
using MediatR;

namespace DataAnalysis.Application.Features.Company.Queries.GetStepDistribution;

public record GetStepDistributionQuery(ProductGroup ProductGroup, string RenewalType) : IRequest<List<StepDistributionResponse>>, IFilteredQuery
{
    public DetailFilter Filter { get; init; } = new();
}
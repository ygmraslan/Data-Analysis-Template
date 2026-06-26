using DataAnalysis.Application.Common.Enums;
using DataAnalysis.Application.Common.Filters;
using MediatR;

namespace DataAnalysis.Application.Features.Demographic.Queries.GetDemoDistribution;

public class GetDemoDistributionQuery : IRequest<List<GetDemoDistributionResponse>>, IFilteredQuery
{
    public ProductGroup ProductGroup { get; set; }
    public DemoDistributionType DistributionType { get; set; }
    public DetailFilter Filter { get; set; } = new();
}
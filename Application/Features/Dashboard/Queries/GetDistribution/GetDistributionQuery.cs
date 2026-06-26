using DataAnalysis.Application.Common.Enums;
using DataAnalysis.Application.Common.Filters;
using MediatR;

namespace DataAnalysis.Application.Features.Dashboard.Queries.GetDistribution;

public class GetDistributionQuery : IRequest<List<GetDistributionQueryResponse>>, IFilteredQuery
{
    public ProductGroup ProductGroup { get; set; }
    public DistributionType DistributionType { get; set; }
    public DetailFilter Filter { get; set; } = new();
}
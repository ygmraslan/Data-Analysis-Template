using DataAnalysis.Application.Common.Enums;
using DataAnalysis.Application.Common.Filters;
using MediatR;

namespace DataAnalysis.Application.Features.Dashboard.Queries.GetSegmentDrift;

public class GetSegmentDriftQuery : IRequest<List<GetSegmentDriftQueryResponse>>, IFilteredQuery
{
    public ProductGroup ProductGroup { get; set; }
    public DetailFilter Filter { get; set; } = new();
}
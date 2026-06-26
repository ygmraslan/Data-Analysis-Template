using DataAnalysis.Application.Common.Enums;
using DataAnalysis.Application.Common.Filters;
using MediatR;

namespace DataAnalysis.Application.Features.Dashboard.Queries.GetWeeklyTotals;

public class GetWeeklyTotalsQuery : IRequest<List<GetWeeklyTotalsQueryResponse>>, IFilteredQuery
{
    public ProductGroup ProductGroup { get; set; }
    public DetailFilter Filter { get; set; } = new();
}
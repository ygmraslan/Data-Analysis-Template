using DataAnalysis.Application.Common.Enums;
using MediatR;

namespace DataAnalysis.Application.Features.ExecSummary.Queries.GetBrandAge;

public class GetBrandAgeQuery : IRequest<GetBrandAgeQueryResponse>
{
    public ProductGroup ProductGroup { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}
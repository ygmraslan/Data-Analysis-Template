using DataAnalysis.Application.Common.Enums;
using MediatR;

namespace DataAnalysis.Application.Features.ExecSummary.Queries.GetDistribution;

public class GetDistributionQuery : IRequest<GetDistributionQueryResponse>
{
    public ProductGroup ProductGroup { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}
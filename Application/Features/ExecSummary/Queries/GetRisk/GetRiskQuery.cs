using DataAnalysis.Application.Common.Enums;
using MediatR;

namespace DataAnalysis.Application.Features.ExecSummary.Queries.GetRisk;

public class GetRiskQuery : IRequest<GetRiskQueryResponse>
{
    public ProductGroup ProductGroup { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}
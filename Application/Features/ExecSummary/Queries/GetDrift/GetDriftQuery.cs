using DataAnalysis.Application.Common.Enums;
using MediatR;

namespace DataAnalysis.Application.Features.ExecSummary.Queries.GetDrift;

public class GetDriftQuery : IRequest<GetDriftQueryResponse>
{
    public ProductGroup ProductGroup { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}
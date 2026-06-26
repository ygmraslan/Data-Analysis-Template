using DataAnalysis.Application.Common.Enums;
using MediatR;

namespace DataAnalysis.Application.Features.ExecSummary.Queries.GetAgeStep;

public class GetAgeStepQuery : IRequest<GetAgeStepQueryResponse>
{
    public ProductGroup ProductGroup { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}
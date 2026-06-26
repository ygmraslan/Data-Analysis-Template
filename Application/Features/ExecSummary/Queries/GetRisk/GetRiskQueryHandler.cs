using DataAnalysis.Application.Features.ExecSummary.Abstractions;
using MediatR;

namespace DataAnalysis.Application.Features.ExecSummary.Queries.GetRisk;

public class GetRiskQueryHandler : IRequestHandler<GetRiskQuery, GetRiskQueryResponse>
{
    private readonly IExecSummaryRepository _repository;

    public GetRiskQueryHandler(IExecSummaryRepository repository)
    {
        _repository = repository;
    }

    public async Task<GetRiskQueryResponse> Handle(GetRiskQuery request, CancellationToken cancellationToken)
    {
        var segments = await _repository.GetRiskSegmentsAsync(
            request.ProductGroup,
            request.StartDate,
            request.EndDate,
            cancellationToken);

        return new GetRiskQueryResponse { Segments = segments };
    }
}
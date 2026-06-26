using DataAnalysis.Application.Features.ExecSummary.Abstractions;
using MediatR;

namespace DataAnalysis.Application.Features.ExecSummary.Queries.GetAgeStep;

public class GetAgeStepQueryHandler : IRequestHandler<GetAgeStepQuery, GetAgeStepQueryResponse>
{
    private readonly IExecSummaryRepository _repository;

    public GetAgeStepQueryHandler(IExecSummaryRepository repository)
    {
        _repository = repository;
    }

    public async Task<GetAgeStepQueryResponse> Handle(GetAgeStepQuery request, CancellationToken cancellationToken)
    {
        var matrix = await _repository.GetAgeStepMatrixAsync(
            request.ProductGroup,
            request.StartDate,
            request.EndDate,
            cancellationToken);

        return new GetAgeStepQueryResponse { Matrix = matrix };
    }
}
using DataAnalysis.Application.Features.ExecSummary.Abstractions;
using MediatR;

namespace DataAnalysis.Application.Features.ExecSummary.Queries.GetBrandAge;

public class GetBrandAgeQueryHandler : IRequestHandler<GetBrandAgeQuery, GetBrandAgeQueryResponse>
{
    private readonly IExecSummaryRepository _repository;

    public GetBrandAgeQueryHandler(IExecSummaryRepository repository)
    {
        _repository = repository;
    }

    public async Task<GetBrandAgeQueryResponse> Handle(GetBrandAgeQuery request, CancellationToken cancellationToken)
    {
        var matrix = await _repository.GetBrandAgeMatrixAsync(
            request.ProductGroup,
            request.StartDate,
            request.EndDate,
            cancellationToken);

        return new GetBrandAgeQueryResponse { Matrix = matrix };
    }
}
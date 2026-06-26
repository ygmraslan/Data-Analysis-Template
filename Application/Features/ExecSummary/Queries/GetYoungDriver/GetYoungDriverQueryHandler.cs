using DataAnalysis.Application.Features.ExecSummary.Abstractions;
using MediatR;

namespace DataAnalysis.Application.Features.ExecSummary.Queries.GetYoungDriver;

public class GetYoungDriverQueryHandler : IRequestHandler<GetYoungDriverQuery, GetYoungDriverQueryResponse>
{
    private readonly IExecSummaryRepository _repository;

    public GetYoungDriverQueryHandler(IExecSummaryRepository repository)
    {
        _repository = repository;
    }

    public async Task<GetYoungDriverQueryResponse> Handle(GetYoungDriverQuery request, CancellationToken cancellationToken)
    {
        var brands = await _repository.GetYoungDriverDistributionAsync(
            request.ProductGroup,
            request.StartDate,
            request.EndDate,
            cancellationToken);

        return new GetYoungDriverQueryResponse { Brands = brands };
    }
}
using DataAnalysis.Application.Features.Company.Abstractions;
using MediatR;

namespace DataAnalysis.Application.Features.Company.Queries.GetStepDistribution;

public class GetStepDistributionHandler : IRequestHandler<GetStepDistributionQuery, List<StepDistributionResponse>>
{
    private readonly ICompanyRepository _repository;

    public GetStepDistributionHandler(ICompanyRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<StepDistributionResponse>> Handle(GetStepDistributionQuery request, CancellationToken cancellationToken)
    {
        var list = await _repository.GetStepDistributionAsync(request.ProductGroup, request.RenewalType, request.Filter, cancellationToken);

        return list.Select(dto => new StepDistributionResponse
        {
            Week        = dto.Week,
            Step        = dto.Step,
            PolicyCount = dto.PolicyCount
        }).ToList();
    }
}
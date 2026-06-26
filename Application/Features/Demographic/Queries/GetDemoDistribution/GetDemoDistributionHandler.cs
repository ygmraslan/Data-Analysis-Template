using DataAnalysis.Application.Common.Enums;
using DataAnalysis.Application.Features.Demographic.Abstractions;
using MediatR;

namespace DataAnalysis.Application.Features.Demographic.Queries.GetDemoDistribution;

public class GetDemoDistributionHandler : IRequestHandler<GetDemoDistributionQuery, List<GetDemoDistributionResponse>>
{
    private readonly IDemoRepository _repository;

    public GetDemoDistributionHandler(IDemoRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<GetDemoDistributionResponse>> Handle(GetDemoDistributionQuery request, CancellationToken cancellationToken)
    {
        var result = request.DistributionType switch
        {
            DemoDistributionType.InsuredType => await _repository.GetInsuredTypeAsync(request.ProductGroup, request.Filter, cancellationToken),
            DemoDistributionType.Gender => await _repository.GetGenderAsync(request.ProductGroup, request.Filter, cancellationToken),
            DemoDistributionType.AgeGroup => await _repository.GetAgeGroupAsync(request.ProductGroup, request.Filter, cancellationToken),
            DemoDistributionType.InsuredCity => await _repository.GetInsuredCityAsync(request.ProductGroup, request.Filter, cancellationToken),
            _ => throw new ArgumentException("Invalid distribution type.")
        };

        return result.Select(x => new GetDemoDistributionResponse
        {
            Label = x.Label,
            PolicyCount = x.PolicyCount,
            NetPremium = x.NetPremium,
            AvgPremium = x.AvgPremium,
            Ratio = x.Ratio,
            WoW = x.WoW
        }).ToList();
    }
}
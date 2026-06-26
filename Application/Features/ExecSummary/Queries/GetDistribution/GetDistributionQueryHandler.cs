using DataAnalysis.Application.Features.ExecSummary.Abstractions;
using MediatR;

namespace DataAnalysis.Application.Features.ExecSummary.Queries.GetDistribution;

public class GetDistributionQueryHandler : IRequestHandler<GetDistributionQuery, GetDistributionQueryResponse>
{
    private readonly IExecSummaryRepository _repository;

    public GetDistributionQueryHandler(IExecSummaryRepository repository)
    {
        _repository = repository;
    }

    public async Task<GetDistributionQueryResponse> Handle(GetDistributionQuery request, CancellationToken cancellationToken)
    {
        var brandsTask = _repository.GetBrandDistributionAsync(
            request.ProductGroup, request.StartDate, request.EndDate, cancellationToken);
        
        var vehicleAgesTask = _repository.GetVehicleAgeDistributionAsync(
            request.ProductGroup, request.StartDate, request.EndDate, cancellationToken);
        
        var stepsTask = _repository.GetStepDistributionAsync(
            request.ProductGroup, request.StartDate, request.EndDate, cancellationToken);
        
        var insuredAgesTask = _repository.GetInsuredAgeDistributionAsync(
            request.ProductGroup, request.StartDate, request.EndDate, cancellationToken);

        await Task.WhenAll(brandsTask, vehicleAgesTask, stepsTask, insuredAgesTask);

        return new GetDistributionQueryResponse
        {
            Brands = await brandsTask,
            VehicleAges = await vehicleAgesTask,
            Steps = await stepsTask,
            InsuredAges = await insuredAgesTask
        };
    }
}
using DataAnalysis.Application.Common.Enums;
using DataAnalysis.Application.Features.Dashboard.Abstractions;
using MediatR;

namespace DataAnalysis.Application.Features.Dashboard.Queries.GetDistribution;

public class GetDistributionQueryHandler : IRequestHandler<GetDistributionQuery, List<GetDistributionQueryResponse>>
{
    private readonly IDashboardRepository _dashboardRepository;

    public GetDistributionQueryHandler(IDashboardRepository dashboardRepository)
    {
        _dashboardRepository = dashboardRepository;
    }

    public async Task<List<GetDistributionQueryResponse>> Handle(GetDistributionQuery request, CancellationToken cancellationToken)
    {
        var result = request.DistributionType switch
        {
            DistributionType.Brand => await _dashboardRepository.GetBrandDistributionAsync(request.ProductGroup, request.Filter, cancellationToken),
            DistributionType.Region => await _dashboardRepository.GetRegionDistributionAsync(request.ProductGroup, request.Filter, cancellationToken),
            DistributionType.VehicleAge => await _dashboardRepository.GetVehicleAgeDistributionAsync(request.ProductGroup, request.Filter, cancellationToken),
            DistributionType.InsuredAge => await _dashboardRepository.GetInsuredAgeDistributionAsync(request.ProductGroup, request.Filter, cancellationToken),
            _ => throw new ArgumentException("Invalid distribution type.")
        };

        return result.Select(x => new GetDistributionQueryResponse
        {
            Label = x.Label,
            Count = x.Count,
            Share = x.Share
        }).ToList();
    }
}
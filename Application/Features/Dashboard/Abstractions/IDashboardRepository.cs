using DataAnalysis.Application.Common.Enums;
using DataAnalysis.Application.Common.Filters;
using DataAnalysis.Application.Features.Dashboard.Dtos;

namespace DataAnalysis.Application.Features.Dashboard.Abstractions;

public interface IDashboardRepository
{
    Task<KpiDto> GetKpiAsync(ProductGroup productGroup, DetailFilter filter, CancellationToken cancellationToken = default);

    Task<List<SegmentDriftDto>> GetSegmentDriftAsync(ProductGroup productGroup, DetailFilter filter, CancellationToken cancellationToken = default);

    Task<List<DistributionDto>> GetBrandDistributionAsync(ProductGroup productGroup, DetailFilter filter, CancellationToken cancellationToken = default);

    Task<List<DistributionDto>> GetRegionDistributionAsync(ProductGroup productGroup, DetailFilter filter, CancellationToken cancellationToken = default);

    Task<List<DistributionDto>> GetVehicleAgeDistributionAsync(ProductGroup productGroup, DetailFilter filter, CancellationToken cancellationToken = default);

    Task<List<DistributionDto>> GetInsuredAgeDistributionAsync(ProductGroup productGroup, DetailFilter filter, CancellationToken cancellationToken = default);

    Task<List<HeatmapDto>> GetHeatmapAsync(ProductGroup productGroup, DetailFilter filter, CancellationToken cancellationToken = default);
    Task<List<WeeklyTotalDto>> GetWeeklyTotalsAsync(ProductGroup productGroup, DetailFilter filter, CancellationToken cancellationToken = default);
}
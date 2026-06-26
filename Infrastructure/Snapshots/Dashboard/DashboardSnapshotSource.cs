using DataAnalysis.Application.Common.Enums;
using DataAnalysis.Application.Common.Filters;
using DataAnalysis.Application.Common.Snapshots;
using DataAnalysis.Infrastructure.Repositories;

namespace DataAnalysis.Infrastructure.Snapshots.Dashboard;

public class DashboardSnapshotSource : ISnapshotSource
{
    private readonly DashboardRepository _repository;

    public DashboardSnapshotSource(DashboardRepository repository)
    {
        _repository = repository;
    }

    public async Task BuildAsync(ISnapshotWriter writer, CancellationToken cancellationToken = default)
    {
        foreach (ProductGroup pg in Enum.GetValues<ProductGroup>())
        {
            await writer.WriteAsync(DashboardSnapshotKeys.Kpi(pg),
                await _repository.GetKpiAsync(pg, new DetailFilter(), cancellationToken), cancellationToken);

            await writer.WriteAsync(DashboardSnapshotKeys.SegmentDrift(pg),
                await _repository.GetSegmentDriftAsync(pg, new DetailFilter(), cancellationToken), cancellationToken);

            await writer.WriteAsync(DashboardSnapshotKeys.BrandDistribution(pg),
                await _repository.GetBrandDistributionAsync(pg, new DetailFilter(), cancellationToken), cancellationToken);

            await writer.WriteAsync(DashboardSnapshotKeys.RegionDistribution(pg),
                await _repository.GetRegionDistributionAsync(pg, new DetailFilter(), cancellationToken), cancellationToken);

            await writer.WriteAsync(DashboardSnapshotKeys.VehicleAgeDistribution(pg),
                await _repository.GetVehicleAgeDistributionAsync(pg, new DetailFilter(), cancellationToken), cancellationToken);

            await writer.WriteAsync(DashboardSnapshotKeys.InsuredAgeDistribution(pg),
                await _repository.GetInsuredAgeDistributionAsync(pg, new DetailFilter(), cancellationToken), cancellationToken);

            await writer.WriteAsync(DashboardSnapshotKeys.Heatmap(pg),
                await _repository.GetHeatmapAsync(pg, new DetailFilter(), cancellationToken), cancellationToken);
                
            await writer.WriteAsync(DashboardSnapshotKeys.WeeklyTotals(pg),
                await _repository.GetWeeklyTotalsAsync(pg, new DetailFilter(), cancellationToken), cancellationToken);    
        }
    }
}
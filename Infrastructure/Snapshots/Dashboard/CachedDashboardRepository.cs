using DataAnalysis.Application.Common.Enums;
using DataAnalysis.Application.Common.Filters;
using DataAnalysis.Application.Common.Snapshots;
using DataAnalysis.Application.Features.Dashboard.Abstractions;
using DataAnalysis.Application.Features.Dashboard.Dtos;

namespace DataAnalysis.Infrastructure.Snapshots.Dashboard;

public class CachedDashboardRepository : IDashboardRepository
{
    private readonly IDashboardRepository _inner;
    private readonly ISnapshotReader _reader;

    public CachedDashboardRepository(IDashboardRepository inner, ISnapshotReader reader)
    {
        _inner = inner;
        _reader = reader;
    }

    public Task<KpiDto> GetKpiAsync(ProductGroup productGroup, DetailFilter filter, CancellationToken cancellationToken = default)
    {
        if (!filter.IsEmpty)
            return _inner.GetKpiAsync(productGroup, filter, cancellationToken);

        return GetOrLiveAsync(DashboardSnapshotKeys.Kpi(productGroup),
            c => _inner.GetKpiAsync(productGroup, filter, c), cancellationToken);
    }

    public Task<List<SegmentDriftDto>> GetSegmentDriftAsync(ProductGroup productGroup, DetailFilter filter, CancellationToken cancellationToken = default)
    {
        if (!filter.IsEmpty)
            return _inner.GetSegmentDriftAsync(productGroup, filter, cancellationToken);

        return GetOrLiveAsync(DashboardSnapshotKeys.SegmentDrift(productGroup),
            c => _inner.GetSegmentDriftAsync(productGroup, filter, c), cancellationToken);
    }

    public Task<List<DistributionDto>> GetBrandDistributionAsync(ProductGroup productGroup, DetailFilter filter, CancellationToken cancellationToken = default)
    {
        if (!filter.IsEmpty)
            return _inner.GetBrandDistributionAsync(productGroup, filter, cancellationToken);

        return GetOrLiveAsync(DashboardSnapshotKeys.BrandDistribution(productGroup),
            c => _inner.GetBrandDistributionAsync(productGroup, filter, c), cancellationToken);
    }

    public Task<List<DistributionDto>> GetRegionDistributionAsync(ProductGroup productGroup, DetailFilter filter, CancellationToken cancellationToken = default)
    {
        if (!filter.IsEmpty)
            return _inner.GetRegionDistributionAsync(productGroup, filter, cancellationToken);

        return GetOrLiveAsync(DashboardSnapshotKeys.RegionDistribution(productGroup),
            c => _inner.GetRegionDistributionAsync(productGroup, filter, c), cancellationToken);
    }

    public Task<List<DistributionDto>> GetVehicleAgeDistributionAsync(ProductGroup productGroup, DetailFilter filter, CancellationToken cancellationToken = default)
    {
        if (!filter.IsEmpty)
            return _inner.GetVehicleAgeDistributionAsync(productGroup, filter, cancellationToken);

        return GetOrLiveAsync(DashboardSnapshotKeys.VehicleAgeDistribution(productGroup),
            c => _inner.GetVehicleAgeDistributionAsync(productGroup, filter, c), cancellationToken);
    }

    public Task<List<DistributionDto>> GetInsuredAgeDistributionAsync(ProductGroup productGroup, DetailFilter filter, CancellationToken cancellationToken = default)
    {
        if (!filter.IsEmpty)
            return _inner.GetInsuredAgeDistributionAsync(productGroup, filter, cancellationToken);

        return GetOrLiveAsync(DashboardSnapshotKeys.InsuredAgeDistribution(productGroup),
            c => _inner.GetInsuredAgeDistributionAsync(productGroup, filter, c), cancellationToken);
    }

    public Task<List<HeatmapDto>> GetHeatmapAsync(ProductGroup productGroup, DetailFilter filter, CancellationToken cancellationToken = default)
    {
        if (!filter.IsEmpty)
            return _inner.GetHeatmapAsync(productGroup, filter, cancellationToken);

        return GetOrLiveAsync(DashboardSnapshotKeys.Heatmap(productGroup),
            c => _inner.GetHeatmapAsync(productGroup, filter, c), cancellationToken);
    }

    public Task<List<WeeklyTotalDto>> GetWeeklyTotalsAsync(ProductGroup productGroup, DetailFilter filter, CancellationToken cancellationToken = default)
    {
        if (!filter.IsEmpty)
            return _inner.GetWeeklyTotalsAsync(productGroup, filter, cancellationToken);

        return GetOrLiveAsync(DashboardSnapshotKeys.WeeklyTotals(productGroup),
            c => _inner.GetWeeklyTotalsAsync(productGroup, filter, c), cancellationToken);
    }

    private async Task<T> GetOrLiveAsync<T>(string key, Func<CancellationToken, Task<T>> live, CancellationToken cancellationToken)
        where T : class
    {
        var cached = await _reader.GetAsync<T>(key, cancellationToken);
        return cached ?? await live(cancellationToken);
    }
}
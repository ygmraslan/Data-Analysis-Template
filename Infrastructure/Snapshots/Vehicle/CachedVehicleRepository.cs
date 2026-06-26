using DataAnalysis.Application.Common.Enums;
using DataAnalysis.Application.Common.Filters;
using DataAnalysis.Application.Common.Snapshots;
using DataAnalysis.Application.Features.Vehicle.Abstractions;
using DataAnalysis.Application.Features.Vehicle.Dtos;

namespace DataAnalysis.Infrastructure.Snapshots.Vehicle;

public class CachedVehicleRepository : IVehicleRepository
{
    private readonly IVehicleRepository _inner;
    private readonly ISnapshotReader _reader;
    private readonly ISnapshotLazyCache _lazyCache;

    public CachedVehicleRepository(IVehicleRepository inner, ISnapshotReader reader, ISnapshotLazyCache lazyCache)
    {
        _inner = inner;
        _reader = reader;
        _lazyCache = lazyCache;
    }

    // Weekly snapshot (filtre varsa → live)
    public Task<VehicleKpiDto> GetKpiAsync(ProductGroup productGroup, DetailFilter filter, CancellationToken cancellationToken = default)
    {
        if (!filter.IsEmpty)
            return _inner.GetKpiAsync(productGroup, filter, cancellationToken);

        return GetOrLiveAsync(VehicleSnapshotKeys.Kpi(productGroup),
            c => _inner.GetKpiAsync(productGroup, filter, c), cancellationToken);
    }

    public Task<List<VehicleAgeDto>> GetAgeAsync(ProductGroup productGroup, bool grouped, DetailFilter filter, CancellationToken cancellationToken = default)
    {
        if (!filter.IsEmpty)
            return _inner.GetAgeAsync(productGroup, grouped, filter, cancellationToken);

        return GetOrLiveAsync(VehicleSnapshotKeys.Age(productGroup, grouped),
            c => _inner.GetAgeAsync(productGroup, grouped, filter, c), cancellationToken);
    }

    public Task<List<VehiclePriceDto>> GetPriceAsync(ProductGroup productGroup, DetailFilter filter, CancellationToken cancellationToken = default)
    {
        if (!filter.IsEmpty)
            return _inner.GetPriceAsync(productGroup, filter, cancellationToken);

        return GetOrLiveAsync(VehicleSnapshotKeys.Price(productGroup),
            c => _inner.GetPriceAsync(productGroup, filter, c), cancellationToken);
    }

    public Task<List<VehicleBodyDto>> GetBodyAsync(ProductGroup productGroup, DetailFilter filter, CancellationToken cancellationToken = default)
    {
        if (!filter.IsEmpty)
            return _inner.GetBodyAsync(productGroup, filter, cancellationToken);

        return GetOrLiveAsync(VehicleSnapshotKeys.Body(productGroup),
            c => _inner.GetBodyAsync(productGroup, filter, c), cancellationToken);
    }

    public Task<List<VehicleSegmentDto>> GetSegmentAsync(ProductGroup productGroup, DetailFilter filter, CancellationToken cancellationToken = default)
    {
        if (!filter.IsEmpty)
            return _inner.GetSegmentAsync(productGroup, filter, cancellationToken);

        return GetOrLiveAsync(VehicleSnapshotKeys.Segment(productGroup),
            c => _inner.GetSegmentAsync(productGroup, filter, c), cancellationToken);
    }

    public Task<List<VehicleHeatmapDto>> GetAgeHeatmapAsync(ProductGroup productGroup, DetailFilter filter, CancellationToken cancellationToken = default)
    {
        if (!filter.IsEmpty)
            return _inner.GetAgeHeatmapAsync(productGroup, filter, cancellationToken);

        return GetOrLiveAsync(VehicleSnapshotKeys.AgeHeatmap(productGroup),
            c => _inner.GetAgeHeatmapAsync(productGroup, filter, c), cancellationToken);
    }

    public Task<List<VehicleHeatmapDto>> GetPriceHeatmapAsync(ProductGroup productGroup, DetailFilter filter, CancellationToken cancellationToken = default)
    {
        if (!filter.IsEmpty)
            return _inner.GetPriceHeatmapAsync(productGroup, filter, cancellationToken);

        return GetOrLiveAsync(VehicleSnapshotKeys.PriceHeatmap(productGroup),
            c => _inner.GetPriceHeatmapAsync(productGroup, filter, c), cancellationToken);
    }

    // Drill-down (lazy; filtre varsa → live, cache bypass)
    public Task<List<VehicleTrendDto>> GetAgeTrendAsync(ProductGroup productGroup, string ageGroup, bool grouped, DetailFilter filter, CancellationToken cancellationToken = default)
    {
        if (!filter.IsEmpty)
            return _inner.GetAgeTrendAsync(productGroup, ageGroup, grouped, filter, cancellationToken);

        return _lazyCache.GetOrAddAsync(VehicleSnapshotKeys.AgeTrend(productGroup, ageGroup, grouped),
            c => _inner.GetAgeTrendAsync(productGroup, ageGroup, grouped, filter, c), cancellationToken);
    }

    public Task<List<VehicleTrendDto>> GetPriceTrendAsync(ProductGroup productGroup, string priceRange, DetailFilter filter, CancellationToken cancellationToken = default)
    {
        if (!filter.IsEmpty)
            return _inner.GetPriceTrendAsync(productGroup, priceRange, filter, cancellationToken);

        return _lazyCache.GetOrAddAsync(VehicleSnapshotKeys.PriceTrend(productGroup, priceRange),
            c => _inner.GetPriceTrendAsync(productGroup, priceRange, filter, c), cancellationToken);
    }

    private async Task<T> GetOrLiveAsync<T>(string key, Func<CancellationToken, Task<T>> live, CancellationToken cancellationToken)
        where T : class
    {
        var cached = await _reader.GetAsync<T>(key, cancellationToken);
        return cached ?? await live(cancellationToken);
    }
}
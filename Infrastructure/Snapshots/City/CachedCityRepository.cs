using DataAnalysis.Application.Common.Enums;
using DataAnalysis.Application.Common.Filters;
using DataAnalysis.Application.Common.Snapshots;
using DataAnalysis.Application.Features.City.Abstractions;
using DataAnalysis.Application.Features.City.Dtos;

namespace DataAnalysis.Infrastructure.Snapshots.City;

public class CachedCityRepository : ICityRepository
{
    private readonly ICityRepository _inner;
    private readonly ISnapshotReader _reader;
    private readonly ISnapshotLazyCache _lazyCache;

    public CachedCityRepository(ICityRepository inner, ISnapshotReader reader, ISnapshotLazyCache lazyCache)
    {
        _inner = inner;
        _reader = reader;
        _lazyCache = lazyCache;
    }

    // Weekly snapshot (filtre varsa → live)
    public Task<CityKpiDto> GetKpiAsync(ProductGroup productGroup, DetailFilter filter, CancellationToken cancellationToken = default)
    {
        if (!filter.IsEmpty)
            return _inner.GetKpiAsync(productGroup, filter, cancellationToken);

        return GetOrLiveAsync(CitySnapshotKeys.Kpi(productGroup),
            c => _inner.GetKpiAsync(productGroup, filter, c), cancellationToken);
    }

    public Task<List<CityListDto>> GetListAsync(ProductGroup productGroup, DetailFilter filter, CancellationToken cancellationToken = default)
    {
        if (!filter.IsEmpty)
            return _inner.GetListAsync(productGroup, filter, cancellationToken);

        return GetOrLiveAsync(CitySnapshotKeys.List(productGroup),
            c => _inner.GetListAsync(productGroup, filter, c), cancellationToken);
    }

    public Task<List<CityHeatmapDto>> GetHeatmapAsync(ProductGroup productGroup, DetailFilter filter, CancellationToken cancellationToken = default)
    {
        if (!filter.IsEmpty)
            return _inner.GetHeatmapAsync(productGroup, filter, cancellationToken);

        return GetOrLiveAsync(CitySnapshotKeys.Heatmap(productGroup),
            c => _inner.GetHeatmapAsync(productGroup, filter, c), cancellationToken);
    }

    // Drill-down (lazy; filtre varsa → live, cache bypass)
    public Task<List<CityTrendDto>> GetTrendAsync(ProductGroup productGroup, string city, DetailFilter filter, CancellationToken cancellationToken = default)
    {
        if (!filter.IsEmpty)
            return _inner.GetTrendAsync(productGroup, city, filter, cancellationToken);

        return _lazyCache.GetOrAddAsync(CitySnapshotKeys.Trend(productGroup, city),
            c => _inner.GetTrendAsync(productGroup, city, filter, c), cancellationToken);
    }

    public Task<List<CityTopBrandDto>> GetTopBrandsAsync(ProductGroup productGroup, string city, DetailFilter filter, CancellationToken cancellationToken = default)
    {
        if (!filter.IsEmpty)
            return _inner.GetTopBrandsAsync(productGroup, city, filter, cancellationToken);

        return _lazyCache.GetOrAddAsync(CitySnapshotKeys.TopBrands(productGroup, city),
            c => _inner.GetTopBrandsAsync(productGroup, city, filter, c), cancellationToken);
    }

    public Task<List<CityProfileDto>> GetProfileAsync(ProductGroup productGroup, string city, DetailFilter filter, CancellationToken cancellationToken = default)
    {
        if (!filter.IsEmpty)
            return _inner.GetProfileAsync(productGroup, city, filter, cancellationToken);

        return _lazyCache.GetOrAddAsync(CitySnapshotKeys.Profile(productGroup, city),
            c => _inner.GetProfileAsync(productGroup, city, filter, c), cancellationToken);
    }

    private async Task<T> GetOrLiveAsync<T>(string key, Func<CancellationToken, Task<T>> live, CancellationToken cancellationToken)
        where T : class
    {
        var cached = await _reader.GetAsync<T>(key, cancellationToken);
        return cached ?? await live(cancellationToken);
    }
}
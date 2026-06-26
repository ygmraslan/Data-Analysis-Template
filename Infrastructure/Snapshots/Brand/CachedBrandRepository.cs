using DataAnalysis.Application.Common.Enums;
using DataAnalysis.Application.Common.Filters;
using DataAnalysis.Application.Common.Snapshots;
using DataAnalysis.Application.Features.Brand.Abstractions;
using DataAnalysis.Application.Features.Brand.Dtos;

namespace DataAnalysis.Infrastructure.Snapshots.Brand;

public class CachedBrandRepository : IBrandRepository
{
    private readonly IBrandRepository _inner;
    private readonly ISnapshotReader _reader;
    private readonly ISnapshotLazyCache _lazyCache;

    public CachedBrandRepository(IBrandRepository inner, ISnapshotReader reader, ISnapshotLazyCache lazyCache)
    {
        _inner = inner;
        _reader = reader;
        _lazyCache = lazyCache;
    }

    // Weekly snapshot (filtre varsa → live)
    public Task<BrandKpiDto> GetKpiAsync(ProductGroup productGroup, DetailFilter filter, CancellationToken cancellationToken = default)
    {
        if (!filter.IsEmpty)
            return _inner.GetKpiAsync(productGroup, filter, cancellationToken);

        return GetOrLiveAsync(BrandSnapshotKeys.Kpi(productGroup),
            c => _inner.GetKpiAsync(productGroup, filter, c), cancellationToken);
    }

    public Task<List<BrandListDto>> GetListAsync(ProductGroup productGroup, DetailFilter filter, CancellationToken cancellationToken = default)
    {
        if (!filter.IsEmpty)
            return _inner.GetListAsync(productGroup, filter, cancellationToken);

        return GetOrLiveAsync(BrandSnapshotKeys.List(productGroup),
            c => _inner.GetListAsync(productGroup, filter, c), cancellationToken);
    }

    public Task<List<BrandHeatmapDto>> GetHeatmapAsync(ProductGroup productGroup, DetailFilter filter, CancellationToken cancellationToken = default)
    {
        if (!filter.IsEmpty)
            return _inner.GetHeatmapAsync(productGroup, filter, cancellationToken);

        return GetOrLiveAsync(BrandSnapshotKeys.Heatmap(productGroup),
            c => _inner.GetHeatmapAsync(productGroup, filter, c), cancellationToken);
    }

    // Drill-down (lazy; filtre varsa → live, cache bypass)
    public Task<List<BrandTrendDto>> GetTrendAsync(ProductGroup productGroup, string brand, DetailFilter filter, CancellationToken cancellationToken = default)
    {
        if (!filter.IsEmpty)
            return _inner.GetTrendAsync(productGroup, brand, filter, cancellationToken);

        return _lazyCache.GetOrAddAsync(BrandSnapshotKeys.Trend(productGroup, brand),
            c => _inner.GetTrendAsync(productGroup, brand, filter, c), cancellationToken);
    }

    public Task<List<BrandModelDto>> GetModelsAsync(ProductGroup productGroup, string brand, DetailFilter filter, CancellationToken cancellationToken = default)
    {
        if (!filter.IsEmpty)
            return _inner.GetModelsAsync(productGroup, brand, filter, cancellationToken);

        return _lazyCache.GetOrAddAsync(BrandSnapshotKeys.Models(productGroup, brand),
            c => _inner.GetModelsAsync(productGroup, brand, filter, c), cancellationToken);
    }

    private async Task<T> GetOrLiveAsync<T>(string key, Func<CancellationToken, Task<T>> live, CancellationToken cancellationToken)
        where T : class
    {
        var cached = await _reader.GetAsync<T>(key, cancellationToken);
        return cached ?? await live(cancellationToken);
    }
}
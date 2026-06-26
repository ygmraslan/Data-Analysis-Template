using DataAnalysis.Application.Common.Enums;
using DataAnalysis.Application.Common.Filters;
using DataAnalysis.Application.Common.Snapshots;
using DataAnalysis.Application.Features.Agency.Abstractions;
using DataAnalysis.Application.Features.Agency.Dtos;

namespace DataAnalysis.Infrastructure.Snapshots.Agency;

public class CachedAgencyRepository : IAgencyRepository
{
    private readonly IAgencyRepository _inner;
    private readonly ISnapshotReader _reader;
    private readonly ISnapshotLazyCache _lazyCache;

    public CachedAgencyRepository(IAgencyRepository inner, ISnapshotReader reader, ISnapshotLazyCache lazyCache)
    {
        _inner = inner;
        _reader = reader;
        _lazyCache = lazyCache;
    }

    // Weekly snapshot (filtre varsa → live)
    public Task<AgencyKpiDto> GetKpiAsync(ProductGroup productGroup, DetailFilter filter, CancellationToken cancellationToken = default)
    {
        if (!filter.IsEmpty)
            return _inner.GetKpiAsync(productGroup, filter, cancellationToken);

        return GetOrLiveAsync(AgencySnapshotKeys.Kpi(productGroup),
            c => _inner.GetKpiAsync(productGroup, filter, c), cancellationToken);
    }

    public Task<List<AgencyRegionDto>> GetRegionDistributionAsync(ProductGroup productGroup, DetailFilter filter, CancellationToken cancellationToken = default)
    {
        if (!filter.IsEmpty)
            return _inner.GetRegionDistributionAsync(productGroup, filter, cancellationToken);

        return GetOrLiveAsync(AgencySnapshotKeys.RegionDistribution(productGroup),
            c => _inner.GetRegionDistributionAsync(productGroup, filter, c), cancellationToken);
    }

    // Drill-down (lazy; filtre varsa → live, cache bypass)
    public Task<List<AgencyTrendDto>> GetTrendAsync(ProductGroup productGroup, string agencyCode, DetailFilter filter, CancellationToken cancellationToken = default)
    {
        if (!filter.IsEmpty)
            return _inner.GetTrendAsync(productGroup, agencyCode, filter, cancellationToken);

        return _lazyCache.GetOrAddAsync(AgencySnapshotKeys.Trend(productGroup, agencyCode),
            c => _inner.GetTrendAsync(productGroup, agencyCode, filter, c), cancellationToken);
    }

    public Task<List<AgencyProfileDto>> GetProfileAsync(ProductGroup productGroup, string agencyCode, DetailFilter filter, CancellationToken cancellationToken = default)
    {
        if (!filter.IsEmpty)
            return _inner.GetProfileAsync(productGroup, agencyCode, filter, cancellationToken);

        return _lazyCache.GetOrAddAsync(AgencySnapshotKeys.Profile(productGroup, agencyCode),
            c => _inner.GetProfileAsync(productGroup, agencyCode, filter, c), cancellationToken);
    }

    public Task<List<AgencyTopBrandDto>> GetTopBrandsAsync(ProductGroup productGroup, string agencyCode, DetailFilter filter, CancellationToken cancellationToken = default)
    {
        if (!filter.IsEmpty)
            return _inner.GetTopBrandsAsync(productGroup, agencyCode, filter, cancellationToken);

        return _lazyCache.GetOrAddAsync(AgencySnapshotKeys.TopBrands(productGroup, agencyCode),
            c => _inner.GetTopBrandsAsync(productGroup, agencyCode, filter, c), cancellationToken);
    }

    public Task<List<AgencyListDto>> GetListAsync(ProductGroup productGroup, DetailFilter filter, int page, int pageSize, string? region = null, CancellationToken cancellationToken = default)
    {
        if (!filter.IsEmpty)
            return _inner.GetListAsync(productGroup, filter, page, pageSize, region, cancellationToken);

        return _lazyCache.GetOrAddAsync(AgencySnapshotKeys.List(productGroup, page, pageSize, region),
            c => _inner.GetListAsync(productGroup, filter, page, pageSize, region, c), cancellationToken);
    }

    public Task<int> GetTotalCountAsync(ProductGroup productGroup, DetailFilter filter, string? region = null, CancellationToken cancellationToken = default)
    {
        if (!filter.IsEmpty)
            return _inner.GetTotalCountAsync(productGroup, filter, region, cancellationToken);

        return _lazyCache.GetOrAddAsync(AgencySnapshotKeys.TotalCount(productGroup, region),
            c => _inner.GetTotalCountAsync(productGroup, filter, region, c), cancellationToken);
    }

    public Task<List<AgencyHeatmapDto>> GetHeatmapAsync(ProductGroup productGroup, DetailFilter filter, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        if (!filter.IsEmpty)
            return _inner.GetHeatmapAsync(productGroup, filter, page, pageSize, cancellationToken);

        return _lazyCache.GetOrAddAsync(AgencySnapshotKeys.Heatmap(productGroup, page, pageSize),
            c => _inner.GetHeatmapAsync(productGroup, filter, page, pageSize, c), cancellationToken);
    }

    private async Task<T> GetOrLiveAsync<T>(string key, Func<CancellationToken, Task<T>> live, CancellationToken cancellationToken)
        where T : class
    {
        var cached = await _reader.GetAsync<T>(key, cancellationToken);
        return cached ?? await live(cancellationToken);
    }
}
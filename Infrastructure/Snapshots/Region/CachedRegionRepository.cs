using DataAnalysis.Application.Common.Enums;
using DataAnalysis.Application.Common.Filters;   
using DataAnalysis.Application.Common.Snapshots;
using DataAnalysis.Application.Features.Region.Abstractions;
using DataAnalysis.Application.Features.Region.Dtos;

namespace DataAnalysis.Infrastructure.Snapshots.Region;

public class CachedRegionRepository : IRegionRepository
{
    private readonly IRegionRepository _inner;
    private readonly ISnapshotReader _reader;

    public CachedRegionRepository(IRegionRepository inner, ISnapshotReader reader)
    {
        _inner = inner;
        _reader = reader;
    }

    public Task<RegionKpiDto> GetKpiAsync(ProductGroup productGroup, DetailFilter filter, CancellationToken cancellationToken = default)
    {
        if (!filter.IsEmpty)
            return _inner.GetKpiAsync(productGroup, filter, cancellationToken);

        return GetOrLiveAsync(RegionSnapshotKeys.Kpi(productGroup),
            c => _inner.GetKpiAsync(productGroup, filter, c), cancellationToken);
    }

    public Task<List<RegionTrendDto>> GetTrendAsync(ProductGroup productGroup, DetailFilter filter, CancellationToken cancellationToken = default)
    {
        if (!filter.IsEmpty)
            return _inner.GetTrendAsync(productGroup, filter, cancellationToken);

        return GetOrLiveAsync(RegionSnapshotKeys.Trend(productGroup),
            c => _inner.GetTrendAsync(productGroup, filter, c), cancellationToken);
    }

    public Task<List<RegionHeatmapDto>> GetHeatmapAsync(ProductGroup productGroup, DetailFilter filter, CancellationToken cancellationToken = default)
    {
        if (!filter.IsEmpty)
            return _inner.GetHeatmapAsync(productGroup, filter, cancellationToken);

        return GetOrLiveAsync(RegionSnapshotKeys.Heatmap(productGroup),
            c => _inner.GetHeatmapAsync(productGroup, filter, c), cancellationToken);
    }

    private async Task<T> GetOrLiveAsync<T>(string key, Func<CancellationToken, Task<T>> live, CancellationToken cancellationToken)
        where T : class
    {
        var cached = await _reader.GetAsync<T>(key, cancellationToken);
        return cached ?? await live(cancellationToken);
    }
}
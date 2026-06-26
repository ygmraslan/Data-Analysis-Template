using DataAnalysis.Application.Common.Enums;
using DataAnalysis.Application.Common.Filters;
using DataAnalysis.Application.Common.Snapshots;
using DataAnalysis.Application.Features.Company.Abstractions;
using DataAnalysis.Application.Features.Company.Dtos;

namespace DataAnalysis.Infrastructure.Snapshots.Company;

public class CachedCompanyRepository : ICompanyRepository
{
    private readonly ICompanyRepository _inner;
    private readonly ISnapshotReader _reader;
    private readonly ISnapshotLazyCache _lazyCache;

    public CachedCompanyRepository(ICompanyRepository inner, ISnapshotReader reader, ISnapshotLazyCache lazyCache)
    {
        _inner = inner;
        _reader = reader;
        _lazyCache = lazyCache;
    }

    // Weekly snapshot (filtre varsa → live)
    public Task<CompanyKpiDto> GetKpiAsync(ProductGroup productGroup, DetailFilter filter, CancellationToken cancellationToken = default)
    {
        if (!filter.IsEmpty)
            return _inner.GetKpiAsync(productGroup, filter, cancellationToken);

        return GetOrLiveAsync(CompanySnapshotKeys.Kpi(productGroup),
            c => _inner.GetKpiAsync(productGroup, filter, c), cancellationToken);
    }

    public Task<List<CompanyListDto>> GetListAsync(ProductGroup productGroup, DetailFilter filter, CancellationToken cancellationToken = default)
    {
        if (!filter.IsEmpty)
            return _inner.GetListAsync(productGroup, filter, cancellationToken);

        return GetOrLiveAsync(CompanySnapshotKeys.List(productGroup),
            c => _inner.GetListAsync(productGroup, filter, c), cancellationToken);
    }

    public Task<List<CompanyRenewalDto>> GetRenewalAsync(ProductGroup productGroup, DetailFilter filter, CancellationToken cancellationToken = default)
    {
        if (!filter.IsEmpty)
            return _inner.GetRenewalAsync(productGroup, filter, cancellationToken);

        return GetOrLiveAsync(CompanySnapshotKeys.Renewal(productGroup),
            c => _inner.GetRenewalAsync(productGroup, filter, c), cancellationToken);
    }

    public Task<List<CompanyHeatmapDto>> GetHeatmapAsync(ProductGroup productGroup, DetailFilter filter, CancellationToken cancellationToken = default)
    {
        if (!filter.IsEmpty)
            return _inner.GetHeatmapAsync(productGroup, filter, cancellationToken);

        return GetOrLiveAsync(CompanySnapshotKeys.Heatmap(productGroup),
            c => _inner.GetHeatmapAsync(productGroup, filter, c), cancellationToken);
    }

    // Drill-down (lazy; filtre varsa → live, cache bypass)
    public Task<List<CompanyTrendDto>> GetTrendAsync(ProductGroup productGroup, string company, DetailFilter filter, CancellationToken cancellationToken = default)
    {
        if (!filter.IsEmpty)
            return _inner.GetTrendAsync(productGroup, company, filter, cancellationToken);

        return _lazyCache.GetOrAddAsync(CompanySnapshotKeys.Trend(productGroup, company),
            c => _inner.GetTrendAsync(productGroup, company, filter, c), cancellationToken);
    }

    public Task<List<CompanyTopBrandDto>> GetTopBrandsAsync(ProductGroup productGroup, string company, DetailFilter filter, CancellationToken cancellationToken = default)
    {
        if (!filter.IsEmpty)
            return _inner.GetTopBrandsAsync(productGroup, company, filter, cancellationToken);

        return _lazyCache.GetOrAddAsync(CompanySnapshotKeys.TopBrands(productGroup, company),
            c => _inner.GetTopBrandsAsync(productGroup, company, filter, c), cancellationToken);
    }

    public Task<List<CompanyProfileDto>> GetProfileAsync(ProductGroup productGroup, string company, DetailFilter filter, CancellationToken cancellationToken = default)
    {
        if (!filter.IsEmpty)
            return _inner.GetProfileAsync(productGroup, company, filter, cancellationToken);

        return _lazyCache.GetOrAddAsync(CompanySnapshotKeys.Profile(productGroup, company),
            c => _inner.GetProfileAsync(productGroup, company, filter, c), cancellationToken);
    }

    public Task<List<StepDistributionDto>> GetStepDistributionAsync(ProductGroup productGroup, string renewalType, DetailFilter filter, CancellationToken cancellationToken = default)
    {
        if (!filter.IsEmpty)
            return _inner.GetStepDistributionAsync(productGroup, renewalType, filter, cancellationToken);

        return _lazyCache.GetOrAddAsync(CompanySnapshotKeys.StepDistribution(productGroup, renewalType),
            c => _inner.GetStepDistributionAsync(productGroup, renewalType, filter, c), cancellationToken);
    }

    private async Task<T> GetOrLiveAsync<T>(string key, Func<CancellationToken, Task<T>> live, CancellationToken cancellationToken)
        where T : class
    {
        var cached = await _reader.GetAsync<T>(key, cancellationToken);
        return cached ?? await live(cancellationToken);
    }
}
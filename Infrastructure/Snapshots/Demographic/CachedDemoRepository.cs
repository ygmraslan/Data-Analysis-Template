using DataAnalysis.Application.Common.Enums;
using DataAnalysis.Application.Common.Filters;
using DataAnalysis.Application.Common.Snapshots;
using DataAnalysis.Application.Features.Demographic.Abstractions;
using DataAnalysis.Application.Features.Demographic.Dtos;

namespace DataAnalysis.Infrastructure.Snapshots.Demographic;

public class CachedDemoRepository : IDemoRepository
{
    private readonly IDemoRepository _inner;
    private readonly ISnapshotReader _reader;

    public CachedDemoRepository(IDemoRepository inner, ISnapshotReader reader)
    {
        _inner = inner;
        _reader = reader;
    }

    public Task<DemoKpiDto> GetKpiAsync(ProductGroup productGroup, DetailFilter filter, CancellationToken cancellationToken = default)
    {
        if (!filter.IsEmpty)
            return _inner.GetKpiAsync(productGroup, filter, cancellationToken);

        return GetOrLiveAsync(DemographicSnapshotKeys.Kpi(productGroup),
            c => _inner.GetKpiAsync(productGroup, filter, c), cancellationToken);
    }

    public Task<List<DemoDistributionDto>> GetInsuredTypeAsync(ProductGroup productGroup, DetailFilter filter, CancellationToken cancellationToken = default)
    {
        if (!filter.IsEmpty)
            return _inner.GetInsuredTypeAsync(productGroup, filter, cancellationToken);

        return GetOrLiveAsync(DemographicSnapshotKeys.InsuredType(productGroup),
            c => _inner.GetInsuredTypeAsync(productGroup, filter, c), cancellationToken);
    }

    public Task<List<DemoDistributionDto>> GetGenderAsync(ProductGroup productGroup, DetailFilter filter, CancellationToken cancellationToken = default)
    {
        if (!filter.IsEmpty)
            return _inner.GetGenderAsync(productGroup, filter, cancellationToken);

        return GetOrLiveAsync(DemographicSnapshotKeys.Gender(productGroup),
            c => _inner.GetGenderAsync(productGroup, filter, c), cancellationToken);
    }

    public Task<List<DemoDistributionDto>> GetAgeGroupAsync(ProductGroup productGroup, DetailFilter filter, CancellationToken cancellationToken = default)
    {
        if (!filter.IsEmpty)
            return _inner.GetAgeGroupAsync(productGroup, filter, cancellationToken);

        return GetOrLiveAsync(DemographicSnapshotKeys.AgeGroup(productGroup),
            c => _inner.GetAgeGroupAsync(productGroup, filter, c), cancellationToken);
    }

    public Task<List<DemoDistributionDto>> GetInsuredCityAsync(ProductGroup productGroup, DetailFilter filter, CancellationToken cancellationToken = default)
    {
        if (!filter.IsEmpty)
            return _inner.GetInsuredCityAsync(productGroup, filter, cancellationToken);

        return GetOrLiveAsync(DemographicSnapshotKeys.InsuredCity(productGroup),
            c => _inner.GetInsuredCityAsync(productGroup, filter, c), cancellationToken);
    }

    private async Task<T> GetOrLiveAsync<T>(string key, Func<CancellationToken, Task<T>> live, CancellationToken cancellationToken)
        where T : class
    {
        var cached = await _reader.GetAsync<T>(key, cancellationToken);
        return cached ?? await live(cancellationToken);
    }
}
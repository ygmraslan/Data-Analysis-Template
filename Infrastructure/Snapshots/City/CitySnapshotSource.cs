using DataAnalysis.Application.Common.Enums;
using DataAnalysis.Application.Common.Filters;
using DataAnalysis.Application.Common.Snapshots;
using DataAnalysis.Infrastructure.Repositories;

namespace DataAnalysis.Infrastructure.Snapshots.City;

public class CitySnapshotSource : ISnapshotSource
{
    private readonly CityRepository _repository;

    public CitySnapshotSource(CityRepository repository)
    {
        _repository = repository;
    }

    public async Task BuildAsync(ISnapshotWriter writer, CancellationToken cancellationToken = default)
    {
        foreach (ProductGroup pg in Enum.GetValues<ProductGroup>())
        {
            await writer.WriteAsync(CitySnapshotKeys.Kpi(pg),
                await _repository.GetKpiAsync(pg, new DetailFilter(), cancellationToken), cancellationToken);

            await writer.WriteAsync(CitySnapshotKeys.List(pg),
                await _repository.GetListAsync(pg, new DetailFilter(), cancellationToken), cancellationToken);

            await writer.WriteAsync(CitySnapshotKeys.Heatmap(pg),
                await _repository.GetHeatmapAsync(pg, new DetailFilter(), cancellationToken), cancellationToken);
        }
    }
}
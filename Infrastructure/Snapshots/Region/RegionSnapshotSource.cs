using DataAnalysis.Application.Common.Enums;
using DataAnalysis.Application.Common.Filters;    
using DataAnalysis.Application.Common.Snapshots;
using DataAnalysis.Infrastructure.Repositories;

namespace DataAnalysis.Infrastructure.Snapshots.Region;

public class RegionSnapshotSource : ISnapshotSource
{
    private readonly RegionRepository _repository;

    public RegionSnapshotSource(RegionRepository repository)
    {
        _repository = repository;
    }

    public async Task BuildAsync(ISnapshotWriter writer, CancellationToken cancellationToken = default)
    {
        foreach (ProductGroup pg in Enum.GetValues<ProductGroup>())
        {
            await writer.WriteAsync(RegionSnapshotKeys.Kpi(pg),
                await _repository.GetKpiAsync(pg, new DetailFilter(), cancellationToken), cancellationToken);

            await writer.WriteAsync(RegionSnapshotKeys.Trend(pg),
                await _repository.GetTrendAsync(pg, new DetailFilter(), cancellationToken), cancellationToken);

            await writer.WriteAsync(RegionSnapshotKeys.Heatmap(pg),
                await _repository.GetHeatmapAsync(pg, new DetailFilter(), cancellationToken), cancellationToken);
        }
    }
}
using DataAnalysis.Application.Common.Enums;
using DataAnalysis.Application.Common.Filters;
using DataAnalysis.Application.Common.Snapshots;
using DataAnalysis.Infrastructure.Repositories;

namespace DataAnalysis.Infrastructure.Snapshots.Brand;

public class BrandSnapshotSource : ISnapshotSource
{
    private readonly BrandRepository _repository;

    public BrandSnapshotSource(BrandRepository repository)
    {
        _repository = repository;
    }

    public async Task BuildAsync(ISnapshotWriter writer, CancellationToken cancellationToken = default)
    {
        foreach (ProductGroup pg in Enum.GetValues<ProductGroup>())
        {
            await writer.WriteAsync(BrandSnapshotKeys.Kpi(pg),
                await _repository.GetKpiAsync(pg, new DetailFilter(), cancellationToken), cancellationToken);

            await writer.WriteAsync(BrandSnapshotKeys.List(pg),
                await _repository.GetListAsync(pg, new DetailFilter(), cancellationToken), cancellationToken);

            await writer.WriteAsync(BrandSnapshotKeys.Heatmap(pg),
                await _repository.GetHeatmapAsync(pg, new DetailFilter(), cancellationToken), cancellationToken);
        }
    }
}
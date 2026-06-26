using DataAnalysis.Application.Common.Enums;
using DataAnalysis.Application.Common.Filters;
using DataAnalysis.Application.Common.Snapshots;
using DataAnalysis.Infrastructure.Repositories;

namespace DataAnalysis.Infrastructure.Snapshots.Vehicle;

public class VehicleSnapshotSource : ISnapshotSource
{
    private readonly VehicleRepository _repository;

    public VehicleSnapshotSource(VehicleRepository repository)
    {
        _repository = repository;
    }

    public async Task BuildAsync(ISnapshotWriter writer, CancellationToken cancellationToken = default)
    {
        foreach (ProductGroup pg in Enum.GetValues<ProductGroup>())
        {
            await writer.WriteAsync(VehicleSnapshotKeys.Kpi(pg),
                await _repository.GetKpiAsync(pg, new DetailFilter(), cancellationToken), cancellationToken);

            foreach (var grouped in new[] { false, true })
                await writer.WriteAsync(VehicleSnapshotKeys.Age(pg, grouped),
                    await _repository.GetAgeAsync(pg, grouped, new DetailFilter(), cancellationToken), cancellationToken);

            await writer.WriteAsync(VehicleSnapshotKeys.Price(pg),
                await _repository.GetPriceAsync(pg, new DetailFilter(), cancellationToken), cancellationToken);

            await writer.WriteAsync(VehicleSnapshotKeys.Body(pg),
                await _repository.GetBodyAsync(pg, new DetailFilter(), cancellationToken), cancellationToken);

            await writer.WriteAsync(VehicleSnapshotKeys.Segment(pg),
                await _repository.GetSegmentAsync(pg, new DetailFilter(), cancellationToken), cancellationToken);

            await writer.WriteAsync(VehicleSnapshotKeys.AgeHeatmap(pg),
                await _repository.GetAgeHeatmapAsync(pg, new DetailFilter(), cancellationToken), cancellationToken);

            await writer.WriteAsync(VehicleSnapshotKeys.PriceHeatmap(pg),
                await _repository.GetPriceHeatmapAsync(pg, new DetailFilter(), cancellationToken), cancellationToken);
        }
    }
}
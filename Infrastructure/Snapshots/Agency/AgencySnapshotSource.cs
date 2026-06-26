using DataAnalysis.Application.Common.Enums;
using DataAnalysis.Application.Common.Filters;
using DataAnalysis.Application.Common.Snapshots;
using DataAnalysis.Infrastructure.Repositories;

namespace DataAnalysis.Infrastructure.Snapshots.Agency;

public class AgencySnapshotSource : ISnapshotSource
{
    private readonly AgencyRepository _repository;

    public AgencySnapshotSource(AgencyRepository repository)
    {
        _repository = repository;
    }

    public async Task BuildAsync(ISnapshotWriter writer, CancellationToken cancellationToken = default)
    {
        foreach (ProductGroup pg in Enum.GetValues<ProductGroup>())
        {
            await writer.WriteAsync(AgencySnapshotKeys.Kpi(pg),
                await _repository.GetKpiAsync(pg, new DetailFilter(), cancellationToken), cancellationToken);

            await writer.WriteAsync(AgencySnapshotKeys.RegionDistribution(pg),
                await _repository.GetRegionDistributionAsync(pg, new DetailFilter(), cancellationToken), cancellationToken);
        }
    }
}
using DataAnalysis.Application.Common.Enums;
using DataAnalysis.Application.Common.Filters;
using DataAnalysis.Application.Common.Snapshots;
using DataAnalysis.Infrastructure.Repositories;

namespace DataAnalysis.Infrastructure.Snapshots.Company;

public class CompanySnapshotSource : ISnapshotSource
{
    private readonly CompanyRepository _repository;

    public CompanySnapshotSource(CompanyRepository repository)
    {
        _repository = repository;
    }

    public async Task BuildAsync(ISnapshotWriter writer, CancellationToken cancellationToken = default)
    {
        foreach (ProductGroup pg in Enum.GetValues<ProductGroup>())
        {
            await writer.WriteAsync(CompanySnapshotKeys.Kpi(pg),
                await _repository.GetKpiAsync(pg, new DetailFilter(), cancellationToken), cancellationToken);

            await writer.WriteAsync(CompanySnapshotKeys.List(pg),
                await _repository.GetListAsync(pg, new DetailFilter(), cancellationToken), cancellationToken);

            await writer.WriteAsync(CompanySnapshotKeys.Renewal(pg),
                await _repository.GetRenewalAsync(pg, new DetailFilter(), cancellationToken), cancellationToken);

            await writer.WriteAsync(CompanySnapshotKeys.Heatmap(pg),
                await _repository.GetHeatmapAsync(pg, new DetailFilter(), cancellationToken), cancellationToken);
        }
    }
}
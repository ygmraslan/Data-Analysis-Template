using DataAnalysis.Application.Common.Enums;
using DataAnalysis.Application.Common.Filters;
using DataAnalysis.Application.Common.Snapshots;
using DataAnalysis.Infrastructure.Repositories;

namespace DataAnalysis.Infrastructure.Snapshots.Demographic;

public class DemographicSnapshotSource : ISnapshotSource
{
    private readonly DemoRepository _repository;

    public DemographicSnapshotSource(DemoRepository repository)
    {
        _repository = repository;
    }

    public async Task BuildAsync(ISnapshotWriter writer, CancellationToken cancellationToken = default)
    {
        foreach (ProductGroup pg in Enum.GetValues<ProductGroup>())
        {
            await writer.WriteAsync(DemographicSnapshotKeys.Kpi(pg),
                await _repository.GetKpiAsync(pg, new DetailFilter(), cancellationToken), cancellationToken);

            await writer.WriteAsync(DemographicSnapshotKeys.InsuredType(pg),
                await _repository.GetInsuredTypeAsync(pg, new DetailFilter(), cancellationToken), cancellationToken);

            await writer.WriteAsync(DemographicSnapshotKeys.Gender(pg),
                await _repository.GetGenderAsync(pg, new DetailFilter(), cancellationToken), cancellationToken);

            await writer.WriteAsync(DemographicSnapshotKeys.AgeGroup(pg),
                await _repository.GetAgeGroupAsync(pg, new DetailFilter(), cancellationToken), cancellationToken);

            await writer.WriteAsync(DemographicSnapshotKeys.InsuredCity(pg),
                await _repository.GetInsuredCityAsync(pg, new DetailFilter(), cancellationToken), cancellationToken);
        }
    }
}
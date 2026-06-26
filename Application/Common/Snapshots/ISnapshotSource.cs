namespace DataAnalysis.Application.Common.Snapshots;

public interface ISnapshotSource
{
    Task BuildAsync(ISnapshotWriter writer, CancellationToken cancellationToken = default);
}
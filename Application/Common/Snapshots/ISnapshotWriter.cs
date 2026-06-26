namespace DataAnalysis.Application.Common.Snapshots;

public interface ISnapshotWriter
{
    Task WriteAsync<T>(string logicalKey, T value, CancellationToken cancellationToken = default);
}
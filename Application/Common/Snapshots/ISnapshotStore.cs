namespace DataAnalysis.Application.Common.Snapshots;

public interface ISnapshotStore
{
    string BeginVersion();

    Task WriteAsync<T>(string version, string logicalKey, T value, CancellationToken cancellationToken = default);

    Task<bool> PromoteAsync(string version, CancellationToken cancellationToken = default);
}
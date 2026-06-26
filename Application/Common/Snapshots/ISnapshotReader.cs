namespace DataAnalysis.Application.Common.Snapshots;

public interface ISnapshotReader
{
    Task<T?> GetAsync<T>(string logicalKey, CancellationToken cancellationToken = default);
}
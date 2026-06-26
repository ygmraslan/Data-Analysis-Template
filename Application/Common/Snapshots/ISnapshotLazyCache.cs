namespace DataAnalysis.Application.Common.Snapshots;

public interface ISnapshotLazyCache
{
    Task<T> GetOrAddAsync<T>(
        string logicalKey,
        Func<CancellationToken, Task<T>> factory,
        CancellationToken cancellationToken = default);
}
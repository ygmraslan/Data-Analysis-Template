using System.Collections.Concurrent;
using System.Text.Json;
using DataAnalysis.Application.Common.Snapshots;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace DataAnalysis.Infrastructure.Snapshots;

public class RedisSnapshotLazyCache : ISnapshotLazyCache
{
    private readonly IConnectionMultiplexer _redis;
    private readonly ILogger<RedisSnapshotLazyCache> _logger;
    private readonly ConcurrentDictionary<string, SemaphoreSlim> _gates = new();

    public RedisSnapshotLazyCache(IConnectionMultiplexer redis, ILogger<RedisSnapshotLazyCache> logger)
    {
        _redis = redis;
        _logger = logger;
    }

    public async Task<T> GetOrAddAsync<T>(
        string logicalKey,
        Func<CancellationToken, Task<T>> factory,
        CancellationToken cancellationToken = default)
    {
        if (!_redis.IsConnected)
            return await factory(cancellationToken);

        var db = _redis.GetDatabase();

        var (found, value) = await TryReadAsync<T>(db, logicalKey);
        if (found)
            return value;

        var gate = _gates.GetOrAdd(logicalKey, _ => new SemaphoreSlim(1, 1));
        await gate.WaitAsync(cancellationToken);
        try
        {
            (found, value) = await TryReadAsync<T>(db, logicalKey);
            if (found)
                return value;

            var computed = await factory(cancellationToken);
            await TryWriteAsync(db, logicalKey, computed); 
            return computed;
        }
        finally
        {
            gate.Release();
        }
    }

    private async Task<(bool Found, T Value)> TryReadAsync<T>(IDatabase db, string logicalKey)
    {
        try
        {
            string? version = await db.StringGetAsync(SnapshotKeys.CurrentPointer);
            if (string.IsNullOrEmpty(version))
                return (false, default!);

            var raw = await db.StringGetAsync(SnapshotKeys.Data(version, logicalKey));
            if (!raw.HasValue)
                return (false, default!);

            return (true, JsonSerializer.Deserialize<T>(raw.ToString())!);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Lazy cache okunamadı (key {Key}); canlı hesaplanıyor.", logicalKey);
            return (false, default!);
        }
    }

    private async Task TryWriteAsync<T>(IDatabase db, string logicalKey, T value)
    {
        try
        {
            string? version = await db.StringGetAsync(SnapshotKeys.CurrentPointer);
            if (string.IsNullOrEmpty(version))
                return;

            var dataKey = SnapshotKeys.Data(version, logicalKey);
            await db.StringSetAsync(dataKey, JsonSerializer.Serialize(value));
            await db.SetAddAsync(SnapshotKeys.Index(version), dataKey);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Lazy cache yazılamadı (key {Key}); atlandı.", logicalKey);
        }
    }
}
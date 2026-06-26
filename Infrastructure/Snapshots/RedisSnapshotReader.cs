using System.Text.Json;
using DataAnalysis.Application.Common.Snapshots;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace DataAnalysis.Infrastructure.Snapshots;

public class RedisSnapshotReader : ISnapshotReader
{
    private readonly IConnectionMultiplexer _redis;
    private readonly ILogger<RedisSnapshotReader> _logger;

    public RedisSnapshotReader(IConnectionMultiplexer redis, ILogger<RedisSnapshotReader> logger)
    {
        _redis = redis;
        _logger = logger;
    }

    public async Task<T?> GetAsync<T>(string logicalKey, CancellationToken cancellationToken = default)
    {
        if (!_redis.IsConnected)
            return default;

        try
        {
            var db = _redis.GetDatabase();

            string? version = await db.StringGetAsync(SnapshotKeys.CurrentPointer);
            if (string.IsNullOrEmpty(version))
                return default;

            string? value = await db.StringGetAsync(SnapshotKeys.Data(version, logicalKey));
            if (string.IsNullOrEmpty(value))
                return default;

            return JsonSerializer.Deserialize<T>(value);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Snapshot okunamadı (key {Key}); canlıya düşülüyor.", logicalKey);
            return default;
        }
    }
}
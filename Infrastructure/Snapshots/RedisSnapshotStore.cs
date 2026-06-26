using System.Text.Json;
using DataAnalysis.Application.Common.Settings;
using DataAnalysis.Application.Common.Snapshots;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace DataAnalysis.Infrastructure.Snapshots;

public class RedisSnapshotStore : ISnapshotStore
{
    private readonly IConnectionMultiplexer _redis;
    private readonly TimeSpan _oldVersionGrace;

    public RedisSnapshotStore(IConnectionMultiplexer redis, IOptions<SnapshotSettings> settings)
    {
        _redis = redis;
        _oldVersionGrace = TimeSpan.FromMinutes(settings.Value.OldVersionGraceMinutes);
    }

    public string BeginVersion()
        => DateTime.UtcNow.ToString("yyyyMMddHHmmss");

    public async Task WriteAsync<T>(string version, string logicalKey, T value, CancellationToken cancellationToken = default)
    {
        var db = _redis.GetDatabase();
        var dataKey = SnapshotKeys.Data(version, logicalKey);

        await db.StringSetAsync(dataKey, JsonSerializer.Serialize(value));
        await db.SetAddAsync(SnapshotKeys.Index(version), dataKey);
    }

    public async Task<bool> PromoteAsync(string version, CancellationToken cancellationToken = default)
    {
        var db = _redis.GetDatabase();

        if (await db.SetLengthAsync(SnapshotKeys.Index(version)) == 0)
            return false;

        string? oldVersion = await db.StringGetAsync(SnapshotKeys.CurrentPointer);

        await db.StringSetAsync(SnapshotKeys.CurrentPointer, version);

        if (!string.IsNullOrEmpty(oldVersion) && oldVersion != version)
            await ExpireVersionAsync(db, oldVersion, _oldVersionGrace);

        return true;
    }

    private static async Task ExpireVersionAsync(IDatabase db, string version, TimeSpan ttl)
    {
        var indexKey = SnapshotKeys.Index(version);

        foreach (var member in await db.SetMembersAsync(indexKey))
            await db.KeyExpireAsync((string)member!, ttl);

        await db.KeyExpireAsync(indexKey, ttl);
    }
}
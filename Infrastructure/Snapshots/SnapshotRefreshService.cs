using System.Diagnostics;
using DataAnalysis.Application.Common.Settings;
using DataAnalysis.Application.Common.Snapshots;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DataAnalysis.Infrastructure.Snapshots;

public class SnapshotRefreshService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly SnapshotSettings _settings;
    private readonly ILogger<SnapshotRefreshService> _logger;

    public SnapshotRefreshService(
        IServiceScopeFactory scopeFactory,
        IOptions<SnapshotSettings> settings,
        ILogger<SnapshotRefreshService> logger)
    {
        _scopeFactory = scopeFactory;
        _settings = settings.Value;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!_settings.Enabled)
        {
            _logger.LogInformation("Snapshot refresh is disabled.");
            return;
        }

        if (_settings.RunOnStartup)
            await RunSafelyAsync(stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            var delay = TimeUntilNextRun(_settings.RunDay, _settings.RunTime);
            _logger.LogInformation("Next snapshot run in {Delay}.", delay);

            try { await Task.Delay(delay, stoppingToken); }
            catch (TaskCanceledException) { break; }

            await RunSafelyAsync(stoppingToken);
        }
    }

    private async Task RunSafelyAsync(CancellationToken ct)
    {
        try
        {
            await RunOnceAsync(ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Snapshot build failed; previous snapshot kept.");
        }
    }

    private async Task RunOnceAsync(CancellationToken ct)
    {
        using var scope = _scopeFactory.CreateScope();
        var store = scope.ServiceProvider.GetRequiredService<ISnapshotStore>();
        var sources = scope.ServiceProvider.GetServices<ISnapshotSource>().ToList();

        if (sources.Count == 0)
        {
            _logger.LogWarning("No snapshot sources registered; skipping build.");
            return;
        }

        var version = store.BeginVersion();
        var writer = new VersionSnapshotWriter(store, version);
        var stopwatch = Stopwatch.StartNew();

        _logger.LogInformation("Snapshot build {Version} started with {Count} source(s).", version, sources.Count);

        foreach (var source in sources)
        {
            ct.ThrowIfCancellationRequested();
            await source.BuildAsync(writer, ct);
            _logger.LogInformation("Source {Source} built.", source.GetType().Name);
        }

        var promoted = await store.PromoteAsync(version, ct);
        stopwatch.Stop();

        if (promoted)
            _logger.LogInformation("Snapshot {Version} promoted in {Elapsed}.", version, stopwatch.Elapsed);
        else
            _logger.LogWarning("Snapshot {Version} produced no data; previous snapshot kept.", version);
    }

    private static TimeSpan TimeUntilNextRun(DayOfWeek runDay, string runTime)
    {
        if (!TimeSpan.TryParse(runTime, out var timeOfDay))
            timeOfDay = new TimeSpan(3, 0, 0);

        var now = DateTime.Now;
        int daysUntil = ((int)runDay - (int)now.DayOfWeek + 7) % 7;
        var nextRun = now.Date.AddDays(daysUntil) + timeOfDay;
        if (nextRun <= now)
            nextRun = nextRun.AddDays(7);
        return nextRun - now;
    }

    private sealed class VersionSnapshotWriter : ISnapshotWriter
    {
        private readonly ISnapshotStore _store;
        private readonly string _version;

        public VersionSnapshotWriter(ISnapshotStore store, string version)
        {
            _store = store;
            _version = version;
        }

        public Task WriteAsync<T>(string logicalKey, T value, CancellationToken cancellationToken = default)
            => _store.WriteAsync(_version, logicalKey, value, cancellationToken);
    }
}
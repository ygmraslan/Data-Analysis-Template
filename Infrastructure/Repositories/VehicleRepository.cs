using Dapper;
using DataAnalysis.Application.Common.Enums;
using DataAnalysis.Application.Common.Filters;
using DataAnalysis.Application.Features.Vehicle.Abstractions;
using DataAnalysis.Application.Features.Vehicle.Dtos;
using DataAnalysis.Infrastructure.Octopus;

namespace DataAnalysis.Infrastructure.Repositories;

public class VehicleRepository : IVehicleRepository
{
    private readonly OctopusConnection _octopus;
    private static readonly string Table = OctopusConnection.PolicyTable;

    public VehicleRepository(OctopusConnection octopus)
    {
        _octopus = octopus;
    }

    private static (DateTime thisWeekStart, DateTime lastWeekStart, DateTime twoWeeksAgoStart, DateTime eightWeeksAgoStart, DateTime nineWeeksAgoStart) GetWeekBoundaries()
    {
        var today        = DateTime.Today;
        var dayOfWeek    = (int)today.DayOfWeek;
        var daysToMonday = dayOfWeek == 0 ? 6 : dayOfWeek - 1;

        var thisWeekStart      = today.AddDays(-daysToMonday);
        var lastWeekStart      = thisWeekStart.AddDays(-7);
        var twoWeeksAgoStart   = thisWeekStart.AddDays(-14);
        var eightWeeksAgoStart = lastWeekStart.AddDays(-49);
        var nineWeeksAgoStart  = lastWeekStart.AddDays(-56);

        return (thisWeekStart, lastWeekStart, twoWeeksAgoStart, eightWeeksAgoStart, nineWeeksAgoStart);
    }

    public async Task<VehicleKpiDto> GetKpiAsync(ProductGroup productGroup, DetailFilter filter, CancellationToken cancellationToken = default)
    {
        // SQL REMOVED (template).
        // Purpose: Weekly KPI summary for the vehicle view (buckets, WoW %, headline metrics).
        // Returns: a single VehicleKpiDto row.
        await Task.CompletedTask;
        throw new NotImplementedException("Implement the vehicle KPI query.");
    }

    public async Task<List<VehicleAgeDto>> GetAgeAsync(ProductGroup productGroup, bool grouped, DetailFilter filter, CancellationToken cancellationToken = default)
    {
        // SQL REMOVED (template).
        // Purpose: Distribution by vehicle age (optionally grouped into buckets) with share %.
        await Task.CompletedTask;
        throw new NotImplementedException("Implement the vehicle age-distribution query.");
    }

    public async Task<List<VehiclePriceDto>> GetPriceAsync(ProductGroup productGroup, DetailFilter filter, CancellationToken cancellationToken = default)
    {
        // SQL REMOVED (template).
        // Purpose: Distribution by vehicle price/value band with share %.
        await Task.CompletedTask;
        throw new NotImplementedException("Implement the vehicle price-distribution query.");
    }

    public async Task<List<VehicleBodyDto>> GetBodyAsync(ProductGroup productGroup, DetailFilter filter, CancellationToken cancellationToken = default)
    {
        // SQL REMOVED (template).
        // Purpose: Distribution by vehicle body/type with share %.
        await Task.CompletedTask;
        throw new NotImplementedException("Implement the vehicle body-distribution query.");
    }

    public async Task<List<VehicleSegmentDto>> GetSegmentAsync(ProductGroup productGroup, DetailFilter filter, CancellationToken cancellationToken = default)
    {
        // SQL REMOVED (template).
        // Purpose: Distribution by vehicle segment with share %.
        await Task.CompletedTask;
        throw new NotImplementedException("Implement the vehicle segment-distribution query.");
    }

    public async Task<List<VehicleTrendDto>> GetAgeTrendAsync(ProductGroup productGroup, string ageGroup, bool grouped, DetailFilter filter, CancellationToken cancellationToken = default)
    {
        // SQL REMOVED (template).
        // Purpose: Weekly time-series for a single vehicle-age group/bucket with WoW change.
        await Task.CompletedTask;
        throw new NotImplementedException("Implement the vehicle age-trend query.");
    }

    public async Task<List<VehicleTrendDto>> GetPriceTrendAsync(ProductGroup productGroup, string priceRange, DetailFilter filter, CancellationToken cancellationToken = default)
    {
        // SQL REMOVED (template).
        // Purpose: Weekly time-series for a single price band with WoW change.
        await Task.CompletedTask;
        throw new NotImplementedException("Implement the vehicle price-trend query.");
    }

    public async Task<List<VehicleHeatmapDto>> GetAgeHeatmapAsync(ProductGroup productGroup, DetailFilter filter, CancellationToken cancellationToken = default)
    {
        // SQL REMOVED (template).
        // Purpose: Vehicle-age x week heatmap matrix.
        await Task.CompletedTask;
        throw new NotImplementedException("Implement the vehicle age-heatmap query.");
    }

    public async Task<List<VehicleHeatmapDto>> GetPriceHeatmapAsync(ProductGroup productGroup, DetailFilter filter, CancellationToken cancellationToken = default)
    {
        // SQL REMOVED (template).
        // Purpose: Price-band x week heatmap matrix.
        await Task.CompletedTask;
        throw new NotImplementedException("Implement the vehicle price-heatmap query.");
    }
}

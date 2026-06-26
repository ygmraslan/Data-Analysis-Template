using Dapper;
using DataAnalysis.Application.Common.Enums;
using DataAnalysis.Application.Common.Filters;
using DataAnalysis.Application.Features.City.Abstractions;
using DataAnalysis.Application.Features.City.Dtos;
using DataAnalysis.Infrastructure.Octopus;

namespace DataAnalysis.Infrastructure.Repositories;

public class CityRepository : ICityRepository
{
    private readonly OctopusConnection _octopus;
    private static readonly string Table = OctopusConnection.PolicyTable;

    public CityRepository(OctopusConnection octopus)
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

    public async Task<CityKpiDto> GetKpiAsync(ProductGroup productGroup, DetailFilter filter, CancellationToken cancellationToken = default)
    {
        // SQL REMOVED (template).
        // Purpose: Weekly KPI summary across cities (buckets, WoW %, top/bottom & gainer/loser city).
        // Returns: a single CityKpiDto row.
        await Task.CompletedTask;
        throw new NotImplementedException("Implement the city KPI query.");
    }

    public async Task<List<CityListDto>> GetListAsync(ProductGroup productGroup, DetailFilter filter, CancellationToken cancellationToken = default)
    {
        // SQL REMOVED (template).
        // Purpose: List of cities with current-period totals and share, for the city table.
        await Task.CompletedTask;
        throw new NotImplementedException("Implement the city list query.");
    }

    public async Task<List<CityTrendDto>> GetTrendAsync(ProductGroup productGroup, string city, DetailFilter filter, CancellationToken cancellationToken = default)
    {
        // SQL REMOVED (template).
        // Purpose: Weekly time-series for a single city with week-over-week change.
        await Task.CompletedTask;
        throw new NotImplementedException("Implement the city trend query.");
    }

    public async Task<List<CityTopBrandDto>> GetTopBrandsAsync(ProductGroup productGroup, string city, DetailFilter filter, CancellationToken cancellationToken = default)
    {
        // SQL REMOVED (template).
        // Purpose: Top brands in a single city, ranked by volume, with share %.
        await Task.CompletedTask;
        throw new NotImplementedException("Implement the city top-brands query.");
    }

    public async Task<List<CityProfileDto>> GetProfileAsync(ProductGroup productGroup, string city, DetailFilter filter, CancellationToken cancellationToken = default)
    {
        // SQL REMOVED (template).
        // Purpose: Detailed profile/breakdown for a single city over the period.
        await Task.CompletedTask;
        throw new NotImplementedException("Implement the city profile query.");
    }

    public async Task<List<CityHeatmapDto>> GetHeatmapAsync(ProductGroup productGroup, DetailFilter filter, CancellationToken cancellationToken = default)
    {
        // SQL REMOVED (template).
        // Purpose: City x week heatmap matrix of a measure per city per week.
        await Task.CompletedTask;
        throw new NotImplementedException("Implement the city heatmap query.");
    }
}

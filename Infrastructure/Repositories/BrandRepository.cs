using Dapper;
using DataAnalysis.Application.Common.Enums;
using DataAnalysis.Application.Common.Filters;
using DataAnalysis.Application.Features.Brand.Abstractions;
using DataAnalysis.Application.Features.Brand.Dtos;
using DataAnalysis.Infrastructure.Octopus;

namespace DataAnalysis.Infrastructure.Repositories;

public class BrandRepository : IBrandRepository
{
    private readonly OctopusConnection _octopus;
    private static readonly string Table = OctopusConnection.PolicyTable;

    public BrandRepository(OctopusConnection octopus)
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

    public async Task<BrandKpiDto> GetKpiAsync(ProductGroup productGroup, DetailFilter filter, CancellationToken cancellationToken = default)
    {
        // SQL REMOVED (template).
        // Purpose: Weekly KPI summary across brands (buckets, WoW %, top/bottom & gainer/loser brand).
        // Returns: a single BrandKpiDto row.
        await Task.CompletedTask;
        throw new NotImplementedException("Implement the brand KPI query.");
    }

    public async Task<List<BrandListDto>> GetListAsync(ProductGroup productGroup, DetailFilter filter, CancellationToken cancellationToken = default)
    {
        // SQL REMOVED (template).
        // Purpose: List of brands with current-period totals and share, for the brand table.
        await Task.CompletedTask;
        throw new NotImplementedException("Implement the brand list query.");
    }

    public async Task<List<BrandTrendDto>> GetTrendAsync(ProductGroup productGroup, string brand, DetailFilter filter, CancellationToken cancellationToken = default)
    {
        // SQL REMOVED (template).
        // Purpose: Weekly time-series for a single brand with week-over-week change.
        await Task.CompletedTask;
        throw new NotImplementedException("Implement the brand trend query.");
    }

    public async Task<List<BrandModelDto>> GetModelsAsync(ProductGroup productGroup, string brand, DetailFilter filter, CancellationToken cancellationToken = default)
    {
        // SQL REMOVED (template).
        // Purpose: Distribution of models within a single brand, ranked by volume, with share %.
        await Task.CompletedTask;
        throw new NotImplementedException("Implement the brand models query.");
    }

    public async Task<List<BrandHeatmapDto>> GetHeatmapAsync(ProductGroup productGroup, DetailFilter filter, CancellationToken cancellationToken = default)
    {
        // SQL REMOVED (template).
        // Purpose: Brand x week heatmap matrix of a measure per brand per week.
        await Task.CompletedTask;
        throw new NotImplementedException("Implement the brand heatmap query.");
    }
}

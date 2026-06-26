using Dapper;
using DataAnalysis.Application.Common.Enums;
using DataAnalysis.Application.Common.Filters;
using DataAnalysis.Application.Features.Dashboard.Abstractions;
using DataAnalysis.Application.Features.Dashboard.Dtos;
using DataAnalysis.Infrastructure.Octopus;

namespace DataAnalysis.Infrastructure.Repositories;

public class DashboardRepository : IDashboardRepository
{
    private readonly OctopusConnection _octopus;
    private static readonly string Table = OctopusConnection.PolicyTable;

    public DashboardRepository(OctopusConnection octopus)
    {
        _octopus = octopus;
    }

    private static (DateTime thisWeekStart, DateTime lastWeekStart, DateTime twoWeeksAgoStart, DateTime eightWeeksAgoStart, DateTime nineWeeksAgoStart) GetWeekBoundaries()
    {
        var today = DateTime.Today;
        var dayOfWeek = (int)today.DayOfWeek;
        var daysToMonday = dayOfWeek == 0 ? 6 : dayOfWeek - 1;

        var thisWeekStart      = today.AddDays(-daysToMonday);
        var lastWeekStart      = thisWeekStart.AddDays(-7);
        var twoWeeksAgoStart   = thisWeekStart.AddDays(-14);
        var eightWeeksAgoStart = lastWeekStart.AddDays(-49);  
        var nineWeeksAgoStart  = lastWeekStart.AddDays(-56);  

        return (thisWeekStart, lastWeekStart, twoWeeksAgoStart, eightWeeksAgoStart, nineWeeksAgoStart);
    }

    public async Task<KpiDto> GetKpiAsync(ProductGroup productGroup, DetailFilter filter, CancellationToken cancellationToken = default)
    {
        // SQL REMOVED (template).
        // Purpose: Top-level KPI tiles for the dashboard - totals and WoW change for the active filters.
        await Task.CompletedTask;
        throw new NotImplementedException("Implement the dashboard KPI query.");
    }

    public async Task<List<SegmentDriftDto>> GetSegmentDriftAsync(ProductGroup productGroup, DetailFilter filter, CancellationToken cancellationToken = default)
    {
        // SQL REMOVED (template).
        // Purpose: Weekly share of predefined segments with WoW change and a 4-week rolling average (drift monitoring).
        await Task.CompletedTask;
        throw new NotImplementedException("Implement the dashboard segment-drift query.");
    }

    public async Task<List<DistributionDto>> GetBrandDistributionAsync(ProductGroup productGroup, DetailFilter filter, CancellationToken cancellationToken = default)
    {
        // SQL REMOVED (template).
        // Purpose: Volume distribution by brand with share %.
        await Task.CompletedTask;
        throw new NotImplementedException("Implement the dashboard brand-distribution query.");
    }

    public async Task<List<DistributionDto>> GetRegionDistributionAsync(ProductGroup productGroup, DetailFilter filter, CancellationToken cancellationToken = default)
    {
        // SQL REMOVED (template).
        // Purpose: Volume distribution by region with share %.
        await Task.CompletedTask;
        throw new NotImplementedException("Implement the dashboard region-distribution query.");
    }

    public async Task<List<DistributionDto>> GetVehicleAgeDistributionAsync(ProductGroup productGroup, DetailFilter filter, CancellationToken cancellationToken = default)
    {
        // SQL REMOVED (template).
        // Purpose: Volume distribution by vehicle-age bucket with share %.
        await Task.CompletedTask;
        throw new NotImplementedException("Implement the dashboard vehicle-age-distribution query.");
    }

    public async Task<List<DistributionDto>> GetInsuredAgeDistributionAsync(ProductGroup productGroup, DetailFilter filter, CancellationToken cancellationToken = default)
    {
        // SQL REMOVED (template).
        // Purpose: Volume distribution by insured-age group with share %.
        await Task.CompletedTask;
        throw new NotImplementedException("Implement the dashboard insured-age-distribution query.");
    }

    public async Task<List<HeatmapDto>> GetHeatmapAsync(ProductGroup productGroup, DetailFilter filter, CancellationToken cancellationToken = default)
    {
        // SQL REMOVED (template).
        // Purpose: Dashboard heatmap matrix (dimension x week) of a measure.
        await Task.CompletedTask;
        throw new NotImplementedException("Implement the dashboard heatmap query.");
    }

    public async Task<List<WeeklyTotalDto>> GetWeeklyTotalsAsync(ProductGroup productGroup, DetailFilter filter, CancellationToken cancellationToken = default)
    {
        // SQL REMOVED (template).
        // Purpose: Total measure per week over the recent window (dashboard trend line).
        await Task.CompletedTask;
        throw new NotImplementedException("Implement the dashboard weekly-totals query.");
    }
}

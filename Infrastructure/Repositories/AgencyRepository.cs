using Dapper;
using DataAnalysis.Application.Common.Enums;
using DataAnalysis.Application.Common.Filters;
using DataAnalysis.Application.Features.Agency.Abstractions;
using DataAnalysis.Application.Features.Agency.Dtos;
using DataAnalysis.Infrastructure.Octopus;
namespace DataAnalysis.Infrastructure.Repositories;

public sealed class AgencyRepository(OctopusConnection octopus) : IAgencyRepository
{
    private readonly OctopusConnection _octopus = octopus;
    private static readonly string Table = OctopusConnection.PolicyTable;

    private static (DateTime thisWeekStart, DateTime lastWeekStart, DateTime twoWeeksAgo, DateTime eightWeeksAgo) GetWeekBoundaries()
    {
        var today = DateTime.Today;
        var dayOfWeek = (int)today.DayOfWeek;
        var daysToMonday = dayOfWeek == 0 ? 6 : dayOfWeek - 1;

        var thisWeekStart = today.AddDays(-daysToMonday);
        var lastWeekStart = thisWeekStart.AddDays(-7);
        var twoWeeksAgo = thisWeekStart.AddDays(-14);
        var eightWeeksAgo = lastWeekStart.AddDays(-49);
        return (thisWeekStart, lastWeekStart, twoWeeksAgo, eightWeeksAgo);
    }

    public async Task<AgencyKpiDto> GetKpiAsync(ProductGroup productGroup, DetailFilter filter, CancellationToken cancellationToken = default)
    {
        // SQL REMOVED (template).
        // Purpose: Weekly KPI summary across agencies.
        //   - Aggregates the source measure into current/previous week buckets, computes WoW %.
        //   - Ranks agencies; returns top/bottom agency by value and top gainer/loser by WoW.
        // Returns: a single AgencyKpiDto row.
        await Task.CompletedTask;
        throw new NotImplementedException("Implement the agency KPI query.");
    }

    public async Task<List<AgencyListDto>> GetListAsync(ProductGroup productGroup, DetailFilter filter, int page, int pageSize, string? region = null, CancellationToken cancellationToken = default)
    {
        // SQL REMOVED (template).
        // Purpose: Paginated list of agencies with current-period totals/metrics
        //   (optionally filtered by region) to populate the agency table.
        await Task.CompletedTask;
        throw new NotImplementedException("Implement the agency list query.");
    }

    public async Task<int> GetTotalCountAsync(ProductGroup productGroup, DetailFilter filter, string? region = null, CancellationToken cancellationToken = default)
    {
        // SQL REMOVED (template).
        // Purpose: Count of distinct agencies matching the active filters (for list pagination).
        await Task.CompletedTask;
        throw new NotImplementedException("Implement the agency count query.");
    }

    public async Task<List<AgencyTrendDto>> GetTrendAsync(ProductGroup productGroup, string agencyCode, DetailFilter filter, CancellationToken cancellationToken = default)
    {
        // SQL REMOVED (template).
        // Purpose: Weekly time-series for a single agency (by code) with week-over-week change.
        await Task.CompletedTask;
        throw new NotImplementedException("Implement the agency trend query.");
    }

    public async Task<List<AgencyProfileDto>> GetProfileAsync(ProductGroup productGroup, string agencyCode, DetailFilter filter, CancellationToken cancellationToken = default)
    {
        // SQL REMOVED (template).
        // Purpose: Detailed profile/breakdown metrics for a single agency over the period.
        await Task.CompletedTask;
        throw new NotImplementedException("Implement the agency profile query.");
    }

    public async Task<List<AgencyTopBrandDto>> GetTopBrandsAsync(ProductGroup productGroup, string agencyCode, DetailFilter filter, CancellationToken cancellationToken = default)
    {
        // SQL REMOVED (template).
        // Purpose: Top brands written by a single agency, ranked by volume, with share %.
        await Task.CompletedTask;
        throw new NotImplementedException("Implement the agency top-brands query.");
    }

    public async Task<List<AgencyRegionDto>> GetRegionDistributionAsync(ProductGroup productGroup, DetailFilter filter, CancellationToken cancellationToken = default)
    {
        // SQL REMOVED (template).
        // Purpose: Distribution of agency volume by region (group-by region) with share %.
        await Task.CompletedTask;
        throw new NotImplementedException("Implement the agency region-distribution query.");
    }

    public async Task<List<AgencyHeatmapDto>> GetHeatmapAsync(ProductGroup productGroup, DetailFilter filter, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        // SQL REMOVED (template).
        // Purpose: Agency x week heatmap matrix (paginated) of a measure per agency per week.
        await Task.CompletedTask;
        throw new NotImplementedException("Implement the agency heatmap query.");
    }
}

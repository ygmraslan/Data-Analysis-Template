using Dapper;
using DataAnalysis.Application.Common.Enums;
using DataAnalysis.Application.Common.Filters;
using DataAnalysis.Application.Features.Company.Abstractions;
using DataAnalysis.Application.Features.Company.Dtos;
using DataAnalysis.Infrastructure.Octopus;

namespace DataAnalysis.Infrastructure.Repositories;

public class CompanyRepository : ICompanyRepository
{
    private readonly OctopusConnection _octopus;
    private static readonly string Table = OctopusConnection.PolicyTable;

    public CompanyRepository(OctopusConnection octopus)
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

    public async Task<CompanyKpiDto> GetKpiAsync(ProductGroup productGroup, DetailFilter filter, CancellationToken cancellationToken = default)
    {
        // SQL REMOVED (template).
        // Purpose: Weekly KPI summary across companies/competitors (buckets, WoW %, top/bottom & gainer/loser).
        // Note: the original design intentionally excludes the business-source filter for this view.
        // Returns: a single CompanyKpiDto row.
        await Task.CompletedTask;
        throw new NotImplementedException("Implement the company KPI query.");
    }

    public async Task<List<CompanyListDto>> GetListAsync(ProductGroup productGroup, DetailFilter filter, CancellationToken cancellationToken = default)
    {
        // SQL REMOVED (template).
        // Purpose: List of companies with current-period totals and share, for the company table.
        await Task.CompletedTask;
        throw new NotImplementedException("Implement the company list query.");
    }

    public async Task<List<CompanyTrendDto>> GetTrendAsync(ProductGroup productGroup, string company, DetailFilter filter, CancellationToken cancellationToken = default)
    {
        // SQL REMOVED (template).
        // Purpose: Weekly time-series for a single company with week-over-week change.
        await Task.CompletedTask;
        throw new NotImplementedException("Implement the company trend query.");
    }

    public async Task<List<CompanyRenewalDto>> GetRenewalAsync(ProductGroup productGroup, DetailFilter filter, CancellationToken cancellationToken = default)
    {
        // SQL REMOVED (template).
        // Purpose: Weekly counts and measure split by movement type (new business / transfer / renewal).
        await Task.CompletedTask;
        throw new NotImplementedException("Implement the company renewal query.");
    }

    public async Task<List<CompanyTopBrandDto>> GetTopBrandsAsync(ProductGroup productGroup, string company, DetailFilter filter, CancellationToken cancellationToken = default)
    {
        // SQL REMOVED (template).
        // Purpose: Top brands for a single company, ranked by volume, with share %.
        await Task.CompletedTask;
        throw new NotImplementedException("Implement the company top-brands query.");
    }

    public async Task<List<CompanyProfileDto>> GetProfileAsync(ProductGroup productGroup, string company, DetailFilter filter, CancellationToken cancellationToken = default)
    {
        // SQL REMOVED (template).
        // Purpose: Detailed profile/breakdown for a single company over the period.
        await Task.CompletedTask;
        throw new NotImplementedException("Implement the company profile query.");
    }

    public async Task<List<CompanyHeatmapDto>> GetHeatmapAsync(ProductGroup productGroup, DetailFilter filter, CancellationToken cancellationToken = default)
    {
        // SQL REMOVED (template).
        // Purpose: Company x week heatmap matrix of a measure per company per week.
        await Task.CompletedTask;
        throw new NotImplementedException("Implement the company heatmap query.");
    }

    public async Task<List<StepDistributionDto>> GetStepDistributionAsync(ProductGroup productGroup, string renewalType, DetailFilter filter, CancellationToken cancellationToken = default)
    {
        // SQL REMOVED (template).
        // Purpose: Distribution of records across discrete tier/step values for a given movement type, with share %.
        await Task.CompletedTask;
        throw new NotImplementedException("Implement the company step-distribution query.");
    }
}

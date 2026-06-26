using Dapper;
using DataAnalysis.Application.Common.Enums;
using DataAnalysis.Application.Common.Filters;
using DataAnalysis.Application.Features.Demographic.Abstractions;
using DataAnalysis.Application.Features.Demographic.Dtos;
using DataAnalysis.Infrastructure.Octopus;

namespace DataAnalysis.Infrastructure.Repositories;

public sealed class DemoRepository(OctopusConnection octopus) : IDemoRepository
{
    private readonly OctopusConnection _octopus = octopus;
    private static readonly string Table = OctopusConnection.PolicyTable;

    private static (DateTime thisWeekStart, DateTime lastWeekStart, DateTime twoWeeksAgo, DateTime threeWeeksAgo) GetWeekBoundaries()
    {
        var today = DateTime.Today;
        var dayOfWeek = (int)today.DayOfWeek;
        var daysToMonday = dayOfWeek == 0 ? 6 : dayOfWeek - 1;

        var thisWeekStart = today.AddDays(-daysToMonday);
        var lastWeekStart = thisWeekStart.AddDays(-7);
        var twoWeeksAgo = thisWeekStart.AddDays(-14);
        var threeWeeksAgo = thisWeekStart.AddDays(-21);
        return (thisWeekStart, lastWeekStart, twoWeeksAgo, threeWeeksAgo);
    }

    public async Task<DemoKpiDto> GetKpiAsync(ProductGroup productGroup, DetailFilter filter, CancellationToken ct = default)
    {
        // SQL REMOVED (template).
        // Purpose: Demographic KPI summary - current/previous buckets and WoW % for the demographic view.
        // Returns: a single DemoKpiDto row.
        await Task.CompletedTask;
        throw new NotImplementedException("Implement the demographic KPI query.");
    }

    public async Task<List<DemoDistributionDto>> GetInsuredTypeAsync(ProductGroup productGroup, DetailFilter filter, CancellationToken ct = default)
    {
        // SQL REMOVED (template).
        // Purpose: Distribution by insured type (e.g. individual vs corporate) with share %.
        await Task.CompletedTask;
        throw new NotImplementedException("Implement the insured-type distribution query.");
    }

    public async Task<List<DemoDistributionDto>> GetGenderAsync(ProductGroup productGroup, DetailFilter filter, CancellationToken ct = default)
    {
        // SQL REMOVED (template).
        // Purpose: Distribution by gender with share %.
        await Task.CompletedTask;
        throw new NotImplementedException("Implement the gender distribution query.");
    }

    public async Task<List<DemoDistributionDto>> GetAgeGroupAsync(ProductGroup productGroup, DetailFilter filter, CancellationToken ct = default)
    {
        // SQL REMOVED (template).
        // Purpose: Distribution by insured age group with share %.
        await Task.CompletedTask;
        throw new NotImplementedException("Implement the age-group distribution query.");
    }

    public async Task<List<DemoDistributionDto>> GetInsuredCityAsync(ProductGroup productGroup, DetailFilter filter, CancellationToken ct = default)
    {
        // SQL REMOVED (template).
        // Purpose: Distribution by insured city, ranked, with share %.
        await Task.CompletedTask;
        throw new NotImplementedException("Implement the insured-city distribution query.");
    }
}

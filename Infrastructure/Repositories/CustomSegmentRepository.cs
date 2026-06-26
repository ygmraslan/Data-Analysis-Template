using Dapper;
using DataAnalysis.Application.Common.Enums;
using DataAnalysis.Application.Features.CustomSegment.Abstractions;
using DataAnalysis.Application.Features.CustomSegment.Dtos;
using DataAnalysis.Infrastructure.Octopus;

namespace DataAnalysis.Infrastructure.Repositories;

public class CustomSegmentRepository : ICustomSegmentRepository
{
    private readonly OctopusConnection _octopus;
    private static readonly string Table = OctopusConnection.PolicyTable;

    public CustomSegmentRepository(OctopusConnection octopus)
    {
        _octopus = octopus;
    }

    public async Task<List<SegmentDriftWeekDto>> CalculateDriftAsync(
        ProductGroup productGroup,
        SegmentFilterDto filters,
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default)
    {
        // SQL REMOVED (template).
        // Purpose: Weekly share time-series for a user-defined segment (built from the supplied filters)
        //   over a date range - i.e. how the custom segment's share drifts week to week.
        // SECURITY: the original built a dynamic WHERE from user input. When implementing, whitelist
        //   allowed columns/values and use parameters to avoid SQL injection.
        await Task.CompletedTask;
        throw new NotImplementedException("Implement the custom-segment drift query.");
    }

    public async Task<SegmentOptionsDto> GetOptionsAsync(
        ProductGroup productGroup,
        CancellationToken cancellationToken = default)
    {
        // SQL REMOVED (template).
        // Purpose: Returns the distinct available values for each filterable dimension
        //   (e.g. brand, insured age, insured type, gender, value band, vehicle age),
        //   used to populate the custom-segment builder dropdowns.
        await Task.CompletedTask;
        throw new NotImplementedException("Implement the segment-options query.");
    }
}

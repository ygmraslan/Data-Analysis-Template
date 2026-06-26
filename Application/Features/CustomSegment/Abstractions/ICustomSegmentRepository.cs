using DataAnalysis.Application.Common.Enums;
using DataAnalysis.Application.Features.CustomSegment.Dtos;

namespace DataAnalysis.Application.Features.CustomSegment.Abstractions;

public interface ICustomSegmentRepository
{
    Task<List<SegmentDriftWeekDto>> CalculateDriftAsync(
        ProductGroup productGroup,
        SegmentFilterDto filters,
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default);

    Task<SegmentOptionsDto> GetOptionsAsync(
        ProductGroup productGroup,
        CancellationToken cancellationToken = default);
}
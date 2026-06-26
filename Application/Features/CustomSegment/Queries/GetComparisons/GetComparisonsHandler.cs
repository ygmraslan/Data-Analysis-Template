using DataAnalysis.Application.Features.CustomSegment.Abstractions;
using DataAnalysis.Application.Features.CustomSegment.Dtos;
using MediatR;

namespace DataAnalysis.Application.Features.CustomSegment.Queries.GetComparisons;

public class GetComparisonsHandler : IRequestHandler<GetComparisonsQuery, GetComparisonsResponse>
{
    private readonly IComparisonRepository _repository;

    public GetComparisonsHandler(IComparisonRepository repository)
    {
        _repository = repository;
    }

    public async Task<GetComparisonsResponse> Handle(
        GetComparisonsQuery request,
        CancellationToken cancellationToken)
    {
        var comparisons = await _repository.GetAllAsync(request.ProductGroup, cancellationToken);

        var items = comparisons.Select(MapToItem).ToList();

        return new GetComparisonsResponse { Items = items };
    }

    private static GetComparisonsItem MapToItem(ComparisonSummaryDto dto)
    {
        return new GetComparisonsItem
        {
            Id = dto.Id,
            Name = dto.Name,
            ProductGroup = dto.ProductGroup,
            WeekStart = dto.WeekStart,
            WeekEnd = dto.WeekEnd,
            CreatedDate = dto.CreatedDate,
            CreatedByName = dto.CreatedByName,
            SegmentA = dto.SegmentA != null ? MapToSide(dto.SegmentA) : null,
            SegmentB = dto.SegmentB != null ? MapToSide(dto.SegmentB) : null
        };
    }

    private static GetComparisonsSideItem MapToSide(ComparisonSideSummaryDto dto)
    {
        return new GetComparisonsSideItem
        {
            Brands = dto.Filters.Brands,
            InsuredAges = dto.Filters.InsuredAges,
            InsuredTypes = dto.Filters.InsuredTypes,
            Genders = dto.Filters.Genders,
            VehicleAges = dto.Filters.VehicleAges,
            VehicleValues = dto.Filters.VehicleValues,
            EndShare = dto.EndShare,
            Change = dto.Change
        };
    }
}
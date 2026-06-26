using DataAnalysis.Application.Features.CustomSegment.Abstractions;
using DataAnalysis.Application.Features.CustomSegment.Dtos;
using MediatR;

namespace DataAnalysis.Application.Features.CustomSegment.Queries.GetComparisonById;

public class GetComparisonByIdHandler : IRequestHandler<GetComparisonByIdQuery, GetComparisonByIdResponse?>
{
    private readonly IComparisonRepository _repository;

    public GetComparisonByIdHandler(IComparisonRepository repository)
    {
        _repository = repository;
    }

    public async Task<GetComparisonByIdResponse?> Handle(
        GetComparisonByIdQuery request,
        CancellationToken cancellationToken)
    {
        var comparison = await _repository.GetByIdAsync(request.Id, cancellationToken);

        if (comparison == null) return null;

        return new GetComparisonByIdResponse
        {
            Id = comparison.Id,
            Name = comparison.Name,
            ProductGroup = comparison.ProductGroup,
            WeekStart = comparison.WeekStart,
            WeekEnd = comparison.WeekEnd,
            CreatedDate = comparison.CreatedDate,
            CreatedByName = comparison.CreatedByName,
            AiCommentDeepSeek = comparison.AiCommentDeepSeek,
            AiCommentGemini = comparison.AiCommentGemini,
            AiCommentGpt = comparison.AiCommentGpt,
            SegmentA = comparison.SegmentA != null ? MapToSide(comparison.SegmentA) : null,
            SegmentB = comparison.SegmentB != null ? MapToSide(comparison.SegmentB) : null
        };
    }

    private static GetComparisonByIdSide MapToSide(ComparisonSideDetailDto dto)
    {
        return new GetComparisonByIdSide
        {
            Filters = new GetComparisonByIdFilters
            {
                Brands = dto.Filters.Brands,
                InsuredAges = dto.Filters.InsuredAges,
                InsuredTypes = dto.Filters.InsuredTypes,
                Genders = dto.Filters.Genders,
                VehicleAges = dto.Filters.VehicleAges,
                VehicleValues = dto.Filters.VehicleValues
            },
            TotalPolicy = dto.TotalPolicy,
            SegmentCount = dto.SegmentCount,
            StartShare = dto.StartShare,
            EndShare = dto.EndShare,
            Change = dto.Change,
            GrowthMultiple = dto.GrowthMultiple,
            WeeklyData = dto.WeeklyData
        };
    }
}
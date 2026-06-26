using DataAnalysis.Application.Features.CustomSegment.Abstractions;
using MediatR;

namespace DataAnalysis.Application.Features.CustomSegment.Queries.GetSegmentById;

public class GetSegmentByIdQueryHandler : IRequestHandler<GetSegmentByIdQuery, GetSegmentByIdQueryResponse?>
{
    private readonly ICustomSegmentDbRepository _repository;

    public GetSegmentByIdQueryHandler(ICustomSegmentDbRepository repository)
    {
        _repository = repository;
    }

    public async Task<GetSegmentByIdQueryResponse?> Handle(GetSegmentByIdQuery request, CancellationToken cancellationToken)
    {
        var segment = await _repository.GetByIdAsync(request.Id, cancellationToken);

        if (segment == null)
            return null;

        return new GetSegmentByIdQueryResponse
        {
            Id = segment.Id,
            Name = segment.Name,
            ProductGroup = segment.ProductGroup,
            Filters = new GetSegmentByIdFilters
            {
                Brands = segment.Filters.Brands,
                InsuredAges = segment.Filters.InsuredAges,
                InsuredTypes = segment.Filters.InsuredTypes,
                Genders = segment.Filters.Genders,
                VehicleAges = segment.Filters.VehicleAges,
                VehicleValues = segment.Filters.VehicleValues
            },
            CreatedDate = segment.CreatedDate,
            CreatedByName = segment.CreatedByName,
            ResultCount = segment.ResultCount,
            LastResult = segment.LastResult != null ? new GetSegmentByIdLastResult
            {
                Id = segment.LastResult.Id,
                StartDate = segment.LastResult.StartDate,
                EndDate = segment.LastResult.EndDate,
                TotalPolicy = segment.LastResult.TotalPolicy,
                SegmentCount = segment.LastResult.SegmentCount,
                StartShare = segment.LastResult.StartShare,
                EndShare = segment.LastResult.EndShare,
                Change = segment.LastResult.Change,
                GrowthMultiple = segment.LastResult.GrowthMultiple,
                WeeklyData = segment.LastResult.WeeklyData,
                AiCommentDeepSeek = segment.LastResult.AiCommentDeepSeek,
                AiCommentGemini = segment.LastResult.AiCommentGemini,
                AiCommentGpt = segment.LastResult.AiCommentGpt,
                CreatedDate = segment.LastResult.CreatedDate,
                CreatedByName = segment.LastResult.CreatedByName
            } : null
        };
    }
}
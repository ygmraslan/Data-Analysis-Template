using DataAnalysis.Application.Features.CustomSegment.Abstractions;
using MediatR;

namespace DataAnalysis.Application.Features.CustomSegment.Queries.GetSegments;

public class GetSegmentsQueryHandler : IRequestHandler<GetSegmentsQuery, GetSegmentsQueryResponse>
{
    private readonly ICustomSegmentDbRepository _repository;

    public GetSegmentsQueryHandler(ICustomSegmentDbRepository repository)
    {
        _repository = repository;
    }

    public async Task<GetSegmentsQueryResponse> Handle(GetSegmentsQuery request, CancellationToken cancellationToken)
    {
        var segments = await _repository.GetAllAsync(
            request.ProductGroup, 
            request.Search, 
            cancellationToken);

        var items = segments.Select(s => new GetSegmentsItem
        {
            Id = s.Id,
            Name = s.Name,
            ProductGroup = s.ProductGroup,
            Filters = new GetSegmentsFilters
            {
                Brands = s.Filters.Brands,
                InsuredAges = s.Filters.InsuredAges,
                InsuredTypes = s.Filters.InsuredTypes,
                Genders = s.Filters.Genders,
                VehicleAges = s.Filters.VehicleAges,
                VehicleValues = s.Filters.VehicleValues
            },
            CreatedDate = s.CreatedDate,
            CreatedByName = s.CreatedByName,
            ResultCount = s.ResultCount,
            LastResult = s.LastResult != null ? new GetSegmentsLastResult
            {
                Id = s.LastResult.Id,
                StartDate = s.LastResult.StartDate,
                EndDate = s.LastResult.EndDate,
                StartShare = s.LastResult.StartShare,
                EndShare = s.LastResult.EndShare,
                Change = s.LastResult.Change,
                GrowthMultiple = s.LastResult.GrowthMultiple,
                CreatedDate = s.LastResult.CreatedDate,
                CreatedByName = s.LastResult.CreatedByName
            } : null
        }).ToList();

        return new GetSegmentsQueryResponse { Items = items };
    }
}
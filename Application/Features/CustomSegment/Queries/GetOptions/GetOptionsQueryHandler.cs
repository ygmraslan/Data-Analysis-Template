using DataAnalysis.Application.Features.CustomSegment.Abstractions;
using MediatR;

namespace DataAnalysis.Application.Features.CustomSegment.Queries.GetOptions;

public class GetOptionsQueryHandler : IRequestHandler<GetOptionsQuery, GetOptionsQueryResponse>
{
    private readonly ICustomSegmentRepository _repository;

    public GetOptionsQueryHandler(ICustomSegmentRepository repository)
    {
        _repository = repository;
    }

    public async Task<GetOptionsQueryResponse> Handle(GetOptionsQuery request, CancellationToken cancellationToken)
    {
        var options = await _repository.GetOptionsAsync(request.ProductGroup, cancellationToken);

        return new GetOptionsQueryResponse
        {
            Brands = options.Brands,
            InsuredAges = options.InsuredAges,
            InsuredTypes = options.InsuredTypes,
            Genders = options.Genders,
            VehicleAges = options.VehicleAges,
            VehicleValues = options.VehicleValues
        };
    }
}
using DataAnalysis.Application.Features.Company.Abstractions;
using MediatR;

namespace DataAnalysis.Application.Features.Company.Queries.GetCompanyHeatmap;

public class GetCompanyHeatmapHandler : IRequestHandler<GetCompanyHeatmapQuery, List<GetCompanyHeatmapResponse>>
{
    private readonly ICompanyRepository _repository;

    public GetCompanyHeatmapHandler(ICompanyRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<GetCompanyHeatmapResponse>> Handle(GetCompanyHeatmapQuery request, CancellationToken cancellationToken)
    {
        var list = await _repository.GetHeatmapAsync(request.ProductGroup, request.Filter, cancellationToken);

        return list.Select(dto => new GetCompanyHeatmapResponse
        {
            Company = dto.Company,
            Week = dto.Week,
            AvgNetPremium = dto.AvgNetPremium,
            PolicyRatio   = dto.PolicyRatio
        }).ToList();
    }
}
using DataAnalysis.Application.Features.Company.Abstractions;
using MediatR;

namespace DataAnalysis.Application.Features.Company.Queries.GetCompanyList;

public class GetCompanyListHandler : IRequestHandler<GetCompanyListQuery, List<GetCompanyListResponse>>
{
    private readonly ICompanyRepository _repository;

    public GetCompanyListHandler(ICompanyRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<GetCompanyListResponse>> Handle(GetCompanyListQuery request, CancellationToken cancellationToken)
    {
        var list = await _repository.GetListAsync(request.ProductGroup, request.Filter, cancellationToken);

        return list.Select(dto => new GetCompanyListResponse
        {
            Company = dto.Company,
            PolicyCount = dto.PolicyCount,
            NetPremium = dto.NetPremium,
            AvgPremium = dto.AvgPremium,
            WoW = dto.WoW
        }).ToList();
    }
}
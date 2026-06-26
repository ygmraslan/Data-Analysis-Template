using DataAnalysis.Application.Features.Company.Abstractions;
using MediatR;

namespace DataAnalysis.Application.Features.Company.Queries.GetCompanyTrend;

public class GetCompanyTrendHandler : IRequestHandler<GetCompanyTrendQuery, List<GetCompanyTrendResponse>>
{
    private readonly ICompanyRepository _repository;

    public GetCompanyTrendHandler(ICompanyRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<GetCompanyTrendResponse>> Handle(GetCompanyTrendQuery request, CancellationToken cancellationToken)
    {
        var list = await _repository.GetTrendAsync(request.ProductGroup, request.Company, request.Filter, cancellationToken);

        return list.Select(dto => new GetCompanyTrendResponse
        {
            WeekLabel = dto.WeekLabel,
            PolicyCount = dto.PolicyCount,
            NetPremium = dto.NetPremium,
            WoW = dto.WoW
        }).ToList();
    }
}
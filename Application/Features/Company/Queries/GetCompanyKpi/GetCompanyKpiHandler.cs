using DataAnalysis.Application.Features.Company.Abstractions;
using MediatR;

namespace DataAnalysis.Application.Features.Company.Queries.GetCompanyKpi;

public class GetCompanyKpiHandler : IRequestHandler<GetCompanyKpiQuery, GetCompanyKpiResponse>
{
    private readonly ICompanyRepository _repository;

    public GetCompanyKpiHandler(ICompanyRepository repository)
    {
        _repository = repository;
    }

    public async Task<GetCompanyKpiResponse> Handle(GetCompanyKpiQuery request, CancellationToken cancellationToken)
    {
        var dto = await _repository.GetKpiAsync(request.ProductGroup, request.Filter, cancellationToken);

        return new GetCompanyKpiResponse
        {
            TopCompanyByCount = dto.TopCompanyByCount,
            TopCompanyCount = dto.TopCompanyCount,
            TopCompanyCountWoW = dto.TopCompanyCountWoW,
            TopCompanyByPremium = dto.TopCompanyByPremium,
            TopCompanyPremium = dto.TopCompanyPremium,
            TopCompanyPremiumWoW = dto.TopCompanyPremiumWoW,
            NewBusinessRatio = dto.NewBusinessRatio,
            NewBusinessRatioWoW = dto.NewBusinessRatioWoW,
            RenewalRatio = dto.RenewalRatio,
            RenewalRatioWoW = dto.RenewalRatioWoW,
            PrevTopCompanyByCount = dto.PrevTopCompanyByCount,
            PrevTopCompanyCount = dto.PrevTopCompanyCount,
            PrevTopCompanyByPremium = dto.PrevTopCompanyByPremium,
            PrevTopCompanyPremium = dto.PrevTopCompanyPremium,
            PrevNewBusinessRatio = dto.PrevNewBusinessRatio,
            PrevRenewalRatio = dto.PrevRenewalRatio,
            DefaultCompany = dto.DefaultCompany
        };
    }
}
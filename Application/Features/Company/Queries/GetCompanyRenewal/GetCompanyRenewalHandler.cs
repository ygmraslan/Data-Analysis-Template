using DataAnalysis.Application.Features.Company.Abstractions;
using MediatR;

namespace DataAnalysis.Application.Features.Company.Queries.GetCompanyRenewal;

public class GetCompanyRenewalHandler : IRequestHandler<GetCompanyRenewalQuery, List<GetCompanyRenewalResponse>>
{
    private readonly ICompanyRepository _repository;

    public GetCompanyRenewalHandler(ICompanyRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<GetCompanyRenewalResponse>> Handle(GetCompanyRenewalQuery request, CancellationToken cancellationToken)
    {
        var list = await _repository.GetRenewalAsync(request.ProductGroup, request.Filter, cancellationToken);

        return list.Select(dto => new GetCompanyRenewalResponse
        {
            WeekLabel          = dto.WeekLabel,
            NewBusinessCount   = dto.NewBusinessCount,
            NewBusinessPremium = dto.NewBusinessPremium,
            NewBusinessRatio   = dto.NewBusinessRatio,
            TransferCount      = dto.TransferCount,     
            TransferPremium    = dto.TransferPremium,    
            TransferRatio      = dto.TransferRatio,      
            RenewalCount       = dto.RenewalCount,
            RenewalPremium     = dto.RenewalPremium,
            RenewalRatio       = dto.RenewalRatio
        }).ToList();
    }
}
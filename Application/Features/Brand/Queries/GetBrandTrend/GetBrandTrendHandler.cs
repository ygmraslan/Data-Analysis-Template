using DataAnalysis.Application.Features.Brand.Abstractions;
using MediatR;

namespace DataAnalysis.Application.Features.Brand.Queries.GetBrandTrend;

public class GetBrandTrendHandler : IRequestHandler<GetBrandTrendQuery, List<BrandTrendResponse>>
{
    private readonly IBrandRepository _repository;

    public GetBrandTrendHandler(IBrandRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<BrandTrendResponse>> Handle(GetBrandTrendQuery request, CancellationToken cancellationToken)
    {
        var list = await _repository.GetTrendAsync(request.ProductGroup, request.Brand, request.Filter, cancellationToken);
        return list.Select(x => new BrandTrendResponse
        {
            WeekLabel   = x.WeekLabel,
            PolicyCount = x.PolicyCount,
            NetPremium  = x.NetPremium,
            WoW         = x.WoW,
        }).ToList();
    }
}
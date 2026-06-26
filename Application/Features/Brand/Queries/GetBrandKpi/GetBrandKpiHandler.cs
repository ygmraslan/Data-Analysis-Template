using DataAnalysis.Application.Features.Brand.Abstractions;
using MediatR;

namespace DataAnalysis.Application.Features.Brand.Queries.GetBrandKpi;

public class GetBrandKpiHandler : IRequestHandler<GetBrandKpiQuery, BrandKpiResponse>
{
    private readonly IBrandRepository _repository;

    public GetBrandKpiHandler(IBrandRepository repository)
    {
        _repository = repository;
    }

    public async Task<BrandKpiResponse> Handle(GetBrandKpiQuery request, CancellationToken cancellationToken)
    {
        var dto = await _repository.GetKpiAsync(request.ProductGroup, request.Filter, cancellationToken);
        return new BrandKpiResponse
        {
            TopBrand           = dto.TopBrand,
            TopBrandCount      = dto.TopBrandCount,
            TopBrandPrev       = dto.TopBrandPrev,
            TopBrandPrevCount  = dto.TopBrandPrevCount,
            BottomBrand        = dto.BottomBrand,
            BottomBrandCount   = dto.BottomBrandCount,
            BottomBrandPrev    = dto.BottomBrandPrev,
            BottomBrandPrevCount = dto.BottomBrandPrevCount,
            TopGainerBrand     = dto.TopGainerBrand,
            TopGainerWoW       = dto.TopGainerWoW,
            HasGainer          = dto.HasGainer,
            PrevTopGainerBrand = dto.PrevTopGainerBrand,
            PrevTopGainerWoW   = dto.PrevTopGainerWoW,
            TopLoserBrand      = dto.TopLoserBrand,
            TopLoserWoW        = dto.TopLoserWoW,
            HasLoser           = dto.HasLoser,
            PrevTopLoserBrand  = dto.PrevTopLoserBrand,
            PrevTopLoserWoW    = dto.PrevTopLoserWoW,
            DefaultBrand       = dto.DefaultBrand,
        };
    }
}
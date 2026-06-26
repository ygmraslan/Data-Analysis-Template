using DataAnalysis.Application.Features.Demographic.Abstractions;
using MediatR;

namespace DataAnalysis.Application.Features.Demographic.Queries.GetDemoKpi;

public sealed class GetDemoKpiHandler(IDemoRepository repository)
    : IRequestHandler<GetDemoKpiQuery, GetDemoKpiResponse>
{
    public async Task<GetDemoKpiResponse> Handle(GetDemoKpiQuery request, CancellationToken cancellationToken)
    {
        var dto = await repository.GetKpiAsync(request.ProductGroup, request.Filter, cancellationToken);

        return new GetDemoKpiResponse
        {
            IndividualCount       = dto.IndividualCount,
            IndividualRatio       = dto.IndividualRatio,
            IndividualWoW         = dto.IndividualWoW,

            PrevIndividualCount   = dto.PrevIndividualCount,
            PrevIndividualRatio   = dto.PrevIndividualRatio,
            PrevIndividualWoW     = dto.PrevIndividualWoW,

            CorporateCount        = dto.CorporateCount,
            CorporateRatio        = dto.CorporateRatio,
            CorporateWoW          = dto.CorporateWoW,

            PrevCorporateCount    = dto.PrevCorporateCount,
            PrevCorporateRatio    = dto.PrevCorporateRatio,
            PrevCorporateWoW      = dto.PrevCorporateWoW,

            TopPlateCity          = dto.TopPlateCity,
            TopPlateCityRatio     = dto.TopPlateCityRatio,
            TopPlateCityWoW       = dto.TopPlateCityWoW,

            PrevTopPlateCity      = dto.PrevTopPlateCity,
            PrevTopPlateCityRatio = dto.PrevTopPlateCityRatio,

            DominantAgeGroup      = dto.DominantAgeGroup,
            DominantAgeRatio      = dto.DominantAgeRatio,
            DominantAgeWoW        = dto.DominantAgeWoW,

            PrevDominantAgeGroup  = dto.PrevDominantAgeGroup,
            PrevDominantAgeRatio  = dto.PrevDominantAgeRatio
        };
    }
}
using DataAnalysis.Application.Features.Agency.Abstractions;
using MediatR;

namespace DataAnalysis.Application.Features.Agency.Queries.GetAgencyKpi;

public sealed class GetAgencyKpiHandler(IAgencyRepository repository)
    : IRequestHandler<GetAgencyKpiQuery, GetAgencyKpiResponse>
{
    public async Task<GetAgencyKpiResponse> Handle(GetAgencyKpiQuery request, CancellationToken cancellationToken)
    {
        var dto = await repository.GetKpiAsync(request.ProductGroup, request.Filter, cancellationToken);

        return new GetAgencyKpiResponse
        {
            TopPremiumAgency          = dto.TopPremiumAgency,
            TopPremiumAmount          = dto.TopPremiumAmount,
            PrevTopPremiumAgency      = dto.PrevTopPremiumAgency,
            PrevTopPremiumAmount      = dto.PrevTopPremiumAmount,

            TopAvgPremiumAgency       = dto.TopAvgPremiumAgency,
            TopAvgPremiumAmount       = dto.TopAvgPremiumAmount,
            PrevTopAvgPremiumAgency   = dto.PrevTopAvgPremiumAgency,
            PrevTopAvgPremiumAmount   = dto.PrevTopAvgPremiumAmount,

            ActiveAgencyCount         = dto.ActiveAgencyCount,
            PrevActiveAgencyCount     = dto.PrevActiveAgencyCount,
            ActiveAgencyCountWoW      = dto.ActiveAgencyCountWoW,
            PrevActiveAgencyCountWoW  = dto.PrevActiveAgencyCountWoW,

            AvgPremiumPerAgency       = dto.AvgPremiumPerAgency,
            PrevAvgPremiumPerAgency   = dto.PrevAvgPremiumPerAgency,
            AvgPremiumPerAgencyWoW    = dto.AvgPremiumPerAgencyWoW,
            PrevAvgPremiumPerAgencyWoW = dto.PrevAvgPremiumPerAgencyWoW,

            DefaultAgencyCode         = dto.DefaultAgencyCode
        };
    }
}
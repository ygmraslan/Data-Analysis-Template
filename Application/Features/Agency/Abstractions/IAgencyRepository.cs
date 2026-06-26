using DataAnalysis.Application.Common.Enums;
using DataAnalysis.Application.Common.Filters;
using DataAnalysis.Application.Features.Agency.Dtos;

namespace DataAnalysis.Application.Features.Agency.Abstractions;

public interface IAgencyRepository
{
    Task<AgencyKpiDto> GetKpiAsync(ProductGroup productGroup, DetailFilter filter, CancellationToken cancellationToken = default);
    Task<List<AgencyListDto>> GetListAsync(ProductGroup productGroup, DetailFilter filter, int page, int pageSize, string? region = null, CancellationToken cancellationToken = default);
    Task<int> GetTotalCountAsync(ProductGroup productGroup, DetailFilter filter, string? region = null, CancellationToken cancellationToken = default);
    Task<List<AgencyTrendDto>> GetTrendAsync(ProductGroup productGroup, string agencyCode, DetailFilter filter, CancellationToken cancellationToken = default);
    Task<List<AgencyProfileDto>> GetProfileAsync(ProductGroup productGroup, string agencyCode, DetailFilter filter, CancellationToken cancellationToken = default);
    Task<List<AgencyTopBrandDto>> GetTopBrandsAsync(ProductGroup productGroup, string agencyCode, DetailFilter filter, CancellationToken cancellationToken = default);
    Task<List<AgencyRegionDto>> GetRegionDistributionAsync(ProductGroup productGroup, DetailFilter filter, CancellationToken cancellationToken = default);
    Task<List<AgencyHeatmapDto>> GetHeatmapAsync(ProductGroup productGroup, DetailFilter filter, int page, int pageSize, CancellationToken cancellationToken = default);
}
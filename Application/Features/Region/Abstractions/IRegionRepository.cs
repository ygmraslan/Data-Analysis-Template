using DataAnalysis.Application.Common.Enums;
using DataAnalysis.Application.Common.Filters;
using DataAnalysis.Application.Features.Region.Dtos;

namespace DataAnalysis.Application.Features.Region.Abstractions;

public interface IRegionRepository
{
    Task<RegionKpiDto> GetKpiAsync(ProductGroup productGroup, DetailFilter filter, CancellationToken cancellationToken = default);
    Task<List<RegionTrendDto>>   GetTrendAsync(ProductGroup productGroup, DetailFilter filter, CancellationToken cancellationToken = default);
    Task<List<RegionHeatmapDto>> GetHeatmapAsync(ProductGroup productGroup, DetailFilter filter, CancellationToken cancellationToken = default);
}
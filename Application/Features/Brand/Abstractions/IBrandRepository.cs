using DataAnalysis.Application.Common.Enums;
using DataAnalysis.Application.Common.Filters;
using DataAnalysis.Application.Features.Brand.Dtos;

namespace DataAnalysis.Application.Features.Brand.Abstractions;

public interface IBrandRepository
{
    Task<BrandKpiDto> GetKpiAsync(ProductGroup productGroup, DetailFilter filter, CancellationToken cancellationToken = default);
    Task<List<BrandListDto>> GetListAsync(ProductGroup productGroup, DetailFilter filter, CancellationToken cancellationToken = default);
    Task<List<BrandTrendDto>> GetTrendAsync(ProductGroup productGroup, string brand, DetailFilter filter, CancellationToken cancellationToken = default);
    Task<List<BrandModelDto>> GetModelsAsync(ProductGroup productGroup, string brand, DetailFilter filter, CancellationToken cancellationToken = default);
    Task<List<BrandHeatmapDto>> GetHeatmapAsync(ProductGroup productGroup, DetailFilter filter, CancellationToken cancellationToken = default);
}
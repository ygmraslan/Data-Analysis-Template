using DataAnalysis.Application.Common.Enums;
using DataAnalysis.Application.Common.Filters;
using DataAnalysis.Application.Features.City.Dtos;

namespace DataAnalysis.Application.Features.City.Abstractions;

public interface ICityRepository
{
    Task<CityKpiDto> GetKpiAsync(ProductGroup productGroup, DetailFilter filter, CancellationToken cancellationToken = default);
    Task<List<CityListDto>> GetListAsync(ProductGroup productGroup, DetailFilter filter, CancellationToken cancellationToken = default);
    Task<List<CityTrendDto>> GetTrendAsync(ProductGroup productGroup, string city, DetailFilter filter, CancellationToken cancellationToken = default);
    Task<List<CityTopBrandDto>> GetTopBrandsAsync(ProductGroup productGroup, string city, DetailFilter filter, CancellationToken cancellationToken = default);
    Task<List<CityProfileDto>> GetProfileAsync(ProductGroup productGroup, string city, DetailFilter filter, CancellationToken cancellationToken = default);
    Task<List<CityHeatmapDto>> GetHeatmapAsync(ProductGroup productGroup, DetailFilter filter, CancellationToken cancellationToken = default);
}
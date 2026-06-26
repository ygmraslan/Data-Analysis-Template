using DataAnalysis.Application.Common.Enums;
using DataAnalysis.Application.Common.Filters;
using DataAnalysis.Application.Features.Vehicle.Dtos;

namespace DataAnalysis.Application.Features.Vehicle.Abstractions;

public interface IVehicleRepository
{
    Task<VehicleKpiDto> GetKpiAsync(ProductGroup productGroup, DetailFilter filter, CancellationToken cancellationToken = default);
    Task<List<VehicleAgeDto>> GetAgeAsync(ProductGroup productGroup, bool grouped, DetailFilter filter, CancellationToken cancellationToken = default);
    Task<List<VehiclePriceDto>> GetPriceAsync(ProductGroup productGroup, DetailFilter filter, CancellationToken cancellationToken = default);
    Task<List<VehicleBodyDto>> GetBodyAsync(ProductGroup productGroup, DetailFilter filter, CancellationToken cancellationToken = default);
    Task<List<VehicleSegmentDto>> GetSegmentAsync(ProductGroup productGroup, DetailFilter filter, CancellationToken cancellationToken = default);
    Task<List<VehicleTrendDto>> GetAgeTrendAsync(ProductGroup productGroup, string ageGroup, bool grouped, DetailFilter filter, CancellationToken cancellationToken = default);
    Task<List<VehicleTrendDto>> GetPriceTrendAsync(ProductGroup productGroup, string priceRange, DetailFilter filter, CancellationToken cancellationToken = default);
    Task<List<VehicleHeatmapDto>> GetAgeHeatmapAsync(ProductGroup productGroup, DetailFilter filter, CancellationToken cancellationToken = default);
    Task<List<VehicleHeatmapDto>> GetPriceHeatmapAsync(ProductGroup productGroup, DetailFilter filter, CancellationToken cancellationToken = default);
}
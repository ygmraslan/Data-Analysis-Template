using DataAnalysis.Application.Common.Enums;
using DataAnalysis.Application.Common.Filters;
using DataAnalysis.Application.Features.Demographic.Dtos;

namespace DataAnalysis.Application.Features.Demographic.Abstractions;

public interface IDemoRepository
{
    Task<DemoKpiDto> GetKpiAsync(ProductGroup productGroup, DetailFilter filter, CancellationToken ct = default);
    Task<List<DemoDistributionDto>> GetInsuredTypeAsync(ProductGroup productGroup, DetailFilter filter, CancellationToken ct = default);
    Task<List<DemoDistributionDto>> GetGenderAsync(ProductGroup productGroup, DetailFilter filter, CancellationToken ct = default);
    Task<List<DemoDistributionDto>> GetAgeGroupAsync(ProductGroup productGroup, DetailFilter filter, CancellationToken ct = default);
    Task<List<DemoDistributionDto>> GetInsuredCityAsync(ProductGroup productGroup, DetailFilter filter, CancellationToken ct = default);
}
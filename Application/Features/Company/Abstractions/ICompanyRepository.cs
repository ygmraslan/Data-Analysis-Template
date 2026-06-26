using DataAnalysis.Application.Common.Enums;
using DataAnalysis.Application.Common.Filters;
using DataAnalysis.Application.Features.Company.Dtos;

namespace DataAnalysis.Application.Features.Company.Abstractions;

public interface ICompanyRepository
{
    Task<CompanyKpiDto> GetKpiAsync(ProductGroup productGroup, DetailFilter filter, CancellationToken cancellationToken = default);

    Task<List<CompanyListDto>> GetListAsync(ProductGroup productGroup, DetailFilter filter, CancellationToken cancellationToken = default);

    Task<List<CompanyTrendDto>> GetTrendAsync(ProductGroup productGroup, string company, DetailFilter filter, CancellationToken cancellationToken = default);

    Task<List<CompanyRenewalDto>> GetRenewalAsync(ProductGroup productGroup, DetailFilter filter, CancellationToken cancellationToken = default);

    Task<List<CompanyTopBrandDto>> GetTopBrandsAsync(ProductGroup productGroup, string company, DetailFilter filter, CancellationToken cancellationToken = default);

    Task<List<CompanyProfileDto>> GetProfileAsync(ProductGroup productGroup, string company, DetailFilter filter, CancellationToken cancellationToken = default);

    Task<List<CompanyHeatmapDto>> GetHeatmapAsync(ProductGroup productGroup, DetailFilter filter, CancellationToken cancellationToken = default);

    Task<List<StepDistributionDto>> GetStepDistributionAsync(ProductGroup productGroup, string renewalType, DetailFilter filter, CancellationToken cancellationToken = default);
}
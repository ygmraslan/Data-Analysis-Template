using DataAnalysis.Application.Common.Enums;
using DataAnalysis.Application.Features.ExecSummary.Dtos;

namespace DataAnalysis.Application.Features.ExecSummary.Abstractions;

public interface IExecSummaryRepository
{
    Task<List<DriftWeekDto>> GetDriftAsync(
        ProductGroup productGroup, 
        DateTime startDate, 
        DateTime endDate, 
        CancellationToken cancellationToken = default);

    Task<List<BrandAgeMatrixDto>> GetBrandAgeMatrixAsync(
        ProductGroup productGroup, 
        DateTime startDate, 
        DateTime endDate, 
        CancellationToken cancellationToken = default);

    Task<List<AgeStepMatrixRowDto>> GetAgeStepMatrixAsync(
        ProductGroup productGroup, 
        DateTime startDate, 
        DateTime endDate, 
        CancellationToken cancellationToken = default);

    Task<List<DistributionItemDto>> GetBrandDistributionAsync(
        ProductGroup productGroup, 
        DateTime startDate, 
        DateTime endDate, 
        CancellationToken cancellationToken = default);

    Task<List<DistributionItemDto>> GetVehicleAgeDistributionAsync(
        ProductGroup productGroup, 
        DateTime startDate, 
        DateTime endDate, 
        CancellationToken cancellationToken = default);

    Task<List<DistributionItemDto>> GetStepDistributionAsync(
        ProductGroup productGroup, 
        DateTime startDate, 
        DateTime endDate, 
        CancellationToken cancellationToken = default);

    Task<List<DistributionItemDto>> GetYoungDriverDistributionAsync(
        ProductGroup productGroup, 
        DateTime startDate, 
        DateTime endDate, 
        CancellationToken cancellationToken = default);

    Task<List<DistributionItemDto>> GetInsuredAgeDistributionAsync(
        ProductGroup productGroup, 
        DateTime startDate, 
        DateTime endDate, 
        CancellationToken cancellationToken = default);

    Task<List<RiskSegmentDto>> GetRiskSegmentsAsync(
        ProductGroup productGroup, 
        DateTime startDate, 
        DateTime endDate, 
        CancellationToken cancellationToken = default);
}
using System.Text.Json;
using DataAnalysis.Application.Features.ExecSummary.Abstractions;
using DataAnalysis.Application.Features.ExecSummary.Dtos;
using MediatR;

namespace DataAnalysis.Application.Features.ExecSummary.Queries.GetAiSummary;

public class GetAiSummaryQueryHandler : IRequestHandler<GetAiSummaryQuery, GetAiSummaryQueryResponse>
{
    private readonly IExecSummaryRepository _repository;
    private readonly IExecAiCacheRepository _cacheRepository;
    private readonly IAiSummaryService _aiService;
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public GetAiSummaryQueryHandler(
        IExecSummaryRepository repository,
        IExecAiCacheRepository cacheRepository,
        IAiSummaryService aiService)
    {
        _repository = repository;
        _cacheRepository = cacheRepository;
        _aiService = aiService;
    }

    public async Task<GetAiSummaryQueryResponse> Handle(GetAiSummaryQuery request, CancellationToken cancellationToken)
    {
        var productType = request.ProductGroup.ToString();
        var modelType = request.ModelType.ToString();
        
        if (!request.ForceRefresh)
        {
            var cached = await _cacheRepository.GetAsync(
                request.StartDate, 
                request.EndDate, 
                productType,
                modelType,
                cancellationToken);

            if (cached != null)
            {
                var cachedData = JsonSerializer.Deserialize<ExecAiDto>(cached.SummaryJson, JsonOptions);
                if (cachedData != null)
                {
                    cachedData.CreatedAt = cached.CreatedAt;
                    cachedData.UpdatedAt = cached.UpdatedAt;
                    
                    return new GetAiSummaryQueryResponse 
                    { 
                        Data = new ModelResponseDto
                        {
                            ModelName = modelType,
                            ModelDisplayName = modelType,
                            Data = cachedData,
                            Success = true,
                            DurationMs = 0
                        },
                        FromCache = true 
                    };
                }
            }
        }

        var aiData = await CollectDataAsync(request, cancellationToken);

        var aiResult = await _aiService.GenerateForSingleModelAsync(aiData, request.ModelType, cancellationToken);

        if (aiResult.Success && aiResult.Data != null)
        {
            var summaryJson = JsonSerializer.Serialize(aiResult.Data, JsonOptions);
            
            if (request.ForceRefresh)
            {
                var existing = await _cacheRepository.GetAsync(
                    request.StartDate, 
                    request.EndDate, 
                    productType,
                    modelType,
                    cancellationToken);

                if (existing != null)
                {
                    var updated = await _cacheRepository.UpdateAsync(
                        existing.Id, 
                        summaryJson, 
                        request.UserId, 
                        cancellationToken);
                    aiResult.Data.UpdatedAt = updated.UpdatedAt;
                }
                else
                {
                    var saved = await _cacheRepository.SaveAsync(
                        request.StartDate,
                        request.EndDate,
                        productType,
                        modelType,
                        summaryJson,
                        request.UserId,
                        cancellationToken);
                    aiResult.Data.CreatedAt = saved.CreatedAt;
                }
            }
            else
            {
                var saved = await _cacheRepository.SaveAsync(
                    request.StartDate,
                    request.EndDate,
                    productType,
                    modelType,
                    summaryJson,
                    request.UserId,
                    cancellationToken);
                aiResult.Data.CreatedAt = saved.CreatedAt;
            }
        }

        return new GetAiSummaryQueryResponse { Data = aiResult, FromCache = false };
    }

    private async Task<AiSummaryDataDto> CollectDataAsync(GetAiSummaryQuery request, CancellationToken cancellationToken)
    {
        var driftTask = _repository.GetDriftAsync(request.ProductGroup, request.StartDate, request.EndDate, cancellationToken);
        var brandAgeTask = _repository.GetBrandAgeMatrixAsync(request.ProductGroup, request.StartDate, request.EndDate, cancellationToken);
        var ageStepTask = _repository.GetAgeStepMatrixAsync(request.ProductGroup, request.StartDate, request.EndDate, cancellationToken);
        var brandTask = _repository.GetBrandDistributionAsync(request.ProductGroup, request.StartDate, request.EndDate, cancellationToken);
        var vehicleAgeTask = _repository.GetVehicleAgeDistributionAsync(request.ProductGroup, request.StartDate, request.EndDate, cancellationToken);
        var stepTask = _repository.GetStepDistributionAsync(request.ProductGroup, request.StartDate, request.EndDate, cancellationToken);
        var insuredAgeTask = _repository.GetInsuredAgeDistributionAsync(request.ProductGroup, request.StartDate, request.EndDate, cancellationToken);
        var youngDriverTask = _repository.GetYoungDriverDistributionAsync(request.ProductGroup, request.StartDate, request.EndDate, cancellationToken);
        var riskTask = _repository.GetRiskSegmentsAsync(request.ProductGroup, request.StartDate, request.EndDate, cancellationToken);

        await Task.WhenAll(driftTask, brandAgeTask, ageStepTask, brandTask, vehicleAgeTask, stepTask, insuredAgeTask, youngDriverTask, riskTask);

        var drift = await driftTask;

        return new AiSummaryDataDto
        {
            DriftTrend = drift,
            Seg1StartShare = drift.FirstOrDefault()?.Seg1Share ?? 0,
            Seg1EndShare = drift.LastOrDefault()?.Seg1Share ?? 0,
            Seg2StartShare = drift.FirstOrDefault()?.Seg2Share ?? 0,
            Seg2EndShare = drift.LastOrDefault()?.Seg2Share ?? 0,
            BrandAgeMatrix = await brandAgeTask,
            AgeStepMatrix = await ageStepTask,
            BrandDistribution = await brandTask,
            VehicleAgeDistribution = await vehicleAgeTask,
            StepDistribution = await stepTask,
            InsuredAgeDistribution = await insuredAgeTask,
            YoungDriverDistribution = await youngDriverTask,
            RiskSegments = await riskTask
        };
    }
}
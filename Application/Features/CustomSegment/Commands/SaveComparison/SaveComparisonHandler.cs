using DataAnalysis.Application.Features.CustomSegment.Abstractions;
using DataAnalysis.Application.Features.CustomSegment.Dtos;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DataAnalysis.Application.Features.CustomSegment.Commands.SaveComparison;

public class SaveComparisonHandler : IRequestHandler<SaveComparisonCommand, SaveComparisonResponse>
{
    private readonly IComparisonRepository _repository;
    private readonly ILogger<SaveComparisonHandler> _logger;

    public SaveComparisonHandler(
        IComparisonRepository repository,
        ILogger<SaveComparisonHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<SaveComparisonResponse> Handle(
        SaveComparisonCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var createDto = MapToCreateDto(request);
            var comparisonId = await _repository.CreateAsync(createDto, request.UserId, cancellationToken);

            _logger.LogInformation(
                "Karşılaştırma kaydedildi: ComparisonId={ComparisonId}, UserId={UserId}",
                comparisonId, request.UserId);

            return new SaveComparisonResponse
            {
                ComparisonId = comparisonId,
                Name = request.Name,
                Success = true,
                Message = "Karşılaştırma başarıyla kaydedildi.",
                AiCommentsGenerated = CountAiComments(request.AiComments)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Karşılaştırma kaydedilirken hata: UserId={UserId}", request.UserId);
            return new SaveComparisonResponse
            {
                Success = false,
                Message = "Karşılaştırma kaydedilemedi."
            };
        }
    }
    private static SaveComparisonRequestDto MapToCreateDto(SaveComparisonCommand cmd)
    {
        return new SaveComparisonRequestDto
        {
            Name = cmd.Name,
            ProductGroup = cmd.ProductGroup,
            WeekStart = cmd.WeekStart,
            WeekEnd = cmd.WeekEnd,
            SegmentAFilters = MapFilters(cmd.SegmentA.Filters),
            SegmentAResult = MapResult(cmd.SegmentA.Result),
            SegmentBFilters = MapFilters(cmd.SegmentB.Filters),
            SegmentBResult = MapResult(cmd.SegmentB.Result),
            AiCommentDeepSeek = cmd.AiComments?.DeepSeek,
            AiCommentGemini = cmd.AiComments?.Gemini,
            AiCommentGpt = cmd.AiComments?.Gpt
        };
    }

    private static SegmentFilterDto MapFilters(SaveComparisonFilters f)
    {
        return new SegmentFilterDto
        {
            Brands = f.Brands,
            InsuredAges = f.InsuredAges,
            InsuredTypes = f.InsuredTypes,
            Genders = f.Genders,
            VehicleAges = f.VehicleAges,
            VehicleValues = f.VehicleValues
        };
    }

    private static ComparisonSideResultDto MapResult(SaveComparisonDriftResult r)
    {
        return new ComparisonSideResultDto
        {
            TotalPolicy = r.TotalPolicy,
            SegmentCount = r.SegmentCount,
            StartShare = r.StartShare,
            EndShare = r.EndShare,
            Change = r.Change,
            GrowthMultiple = r.GrowthMultiple,
            WeeklyData = r.WeeklyData ?? new List<SegmentDriftWeekDto>()
        };
    }

    private static int CountAiComments(SaveComparisonAiComments? ai)
    {
        if (ai == null) return 0;
        return (string.IsNullOrWhiteSpace(ai.DeepSeek) ? 0 : 1)
             + (string.IsNullOrWhiteSpace(ai.Gemini) ? 0 : 1)
             + (string.IsNullOrWhiteSpace(ai.Gpt) ? 0 : 1);
    }
}
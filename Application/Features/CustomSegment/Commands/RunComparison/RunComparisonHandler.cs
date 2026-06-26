using DataAnalysis.Application.Common.Enums;
using DataAnalysis.Application.Features.CustomSegment.Abstractions;
using DataAnalysis.Application.Features.CustomSegment.Dtos;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DataAnalysis.Application.Features.CustomSegment.Commands.RunComparison;

public class RunComparisonHandler : IRequestHandler<RunComparisonCommand, RunComparisonResponse>
{
    private readonly IComparisonRepository _comparisonRepository;
    private readonly ICustomSegmentRepository _octopusRepository;
    private readonly ICustomSegmentAiService _aiService;
    private readonly ILogger<RunComparisonHandler> _logger;

    public RunComparisonHandler(
        IComparisonRepository comparisonRepository,
        ICustomSegmentRepository octopusRepository,
        ICustomSegmentAiService aiService,
        ILogger<RunComparisonHandler> logger)
    {
        _comparisonRepository = comparisonRepository;
        _octopusRepository = octopusRepository;
        _aiService = aiService;
        _logger = logger;
    }

    public async Task<RunComparisonResponse> Handle(
        RunComparisonCommand request,
        CancellationToken cancellationToken)
    {
        var comparison = await _comparisonRepository.GetByIdAsync(request.ComparisonId, cancellationToken);
        if (comparison == null)
        {
            return new RunComparisonResponse
            {
                Success = false,
                Message = "Karşılaştırma bulunamadı."
            };
        }

        if (comparison.SegmentA == null || comparison.SegmentB == null)
        {
            return new RunComparisonResponse
            {
                Success = false,
                Message = "Karşılaştırma verisi eksik."
            };
        }

        var startDate = request.WeekStart.AddDays(-49);
        var endDate = request.WeekEnd;
        var productGroup = Enum.Parse<ProductGroup>(comparison.ProductGroup);

        var driftTaskA = _octopusRepository.CalculateDriftAsync(
            productGroup, comparison.SegmentA.Filters, startDate, endDate, cancellationToken);
        var driftTaskB = _octopusRepository.CalculateDriftAsync(
            productGroup, comparison.SegmentB.Filters, startDate, endDate, cancellationToken);

        await Task.WhenAll(driftTaskA, driftTaskB);

        var weeklyA = driftTaskA.Result;
        var weeklyB = driftTaskB.Result;

        if (weeklyA.Count == 0 || weeklyB.Count == 0)
        {
            return new RunComparisonResponse
            {
                Success = false,
                Message = "Seçilen kriterlere uygun veri bulunamadı."
            };
        }

        var sideAResult = BuildSideResult(weeklyA);
        var sideBResult = BuildSideResult(weeklyB);

        await _comparisonRepository.UpdateResultsAsync(
            request.ComparisonId,
            startDate,
            endDate,
            sideAResult,
            sideBResult,
            request.UserId,
            cancellationToken);

        string? deepSeek = null;
        string? gemini = null;
        string? gpt = null;

        try
        {
            var segmentA = BuildSegmentDtoForAi(comparison, comparison.SegmentA);
            var segmentB = BuildSegmentDtoForAi(comparison, comparison.SegmentB);
            var resultA = BuildResultDtoForAi(sideAResult);
            var resultB = BuildResultDtoForAi(sideBResult);

            var deepSeekTask = _aiService.GenerateComparisonCommentAsync(
                segmentA, resultA, segmentB, resultB, AiModelType.DeepSeek, cancellationToken);
            var geminiTask = _aiService.GenerateComparisonCommentAsync(
                segmentA, resultA, segmentB, resultB, AiModelType.Gemini, cancellationToken);
            var gptTask = _aiService.GenerateComparisonCommentAsync(
                segmentA, resultA, segmentB, resultB, AiModelType.Gpt, cancellationToken);

            await Task.WhenAll(deepSeekTask, geminiTask, gptTask);

            deepSeek = deepSeekTask.Result;
            gemini = geminiTask.Result;
            gpt = gptTask.Result;

            await _comparisonRepository.UpdateAiCommentsAsync(
                request.ComparisonId,
                deepSeek,
                gemini,
                gpt,
                request.UserId,
                cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "RunComparison AI yorumu alınırken hata: ComparisonId={ComparisonId}",
                request.ComparisonId);
        }

        return new RunComparisonResponse
        {
            ComparisonId = request.ComparisonId,
            WeekStart = startDate,
            WeekEnd = endDate,
            SegmentA = MapSideToResponse(sideAResult),
            SegmentB = MapSideToResponse(sideBResult),
            AiCommentDeepSeek = deepSeek,
            AiCommentGemini = gemini,
            AiCommentGpt = gpt,
            Success = true
        };
    }

    private static ComparisonSideResultDto BuildSideResult(List<SegmentDriftWeekDto> weekly)
    {
        var firstWeek = weekly.First();
        var lastWeek = weekly.Last();

        return new ComparisonSideResultDto
        {
            TotalPolicy = weekly.Sum(x => x.TotalPolicy),
            SegmentCount = weekly.Sum(x => x.SegmentCount),
            StartShare = firstWeek.SegmentShare,
            EndShare = lastWeek.SegmentShare,
            Change = lastWeek.SegmentShare - firstWeek.SegmentShare,
            GrowthMultiple = firstWeek.SegmentShare > 0
                ? Math.Round(lastWeek.SegmentShare / firstWeek.SegmentShare, 2)
                : 0,
            WeeklyData = weekly
        };
    }

    private static SegmentDto BuildSegmentDtoForAi(ComparisonDetailDto comparison, ComparisonSideDetailDto side)
    {
        return new SegmentDto
        {
            Name = comparison.Name,
            ProductGroup = comparison.ProductGroup,
            Filters = side.Filters
        };
    }

    private static SegmentResultDto BuildResultDtoForAi(ComparisonSideResultDto side)
    {
        return new SegmentResultDto
        {
            TotalPolicy = side.TotalPolicy,
            SegmentCount = side.SegmentCount,
            StartShare = side.StartShare,
            EndShare = side.EndShare,
            Change = side.Change,
            GrowthMultiple = side.GrowthMultiple,
            WeeklyData = side.WeeklyData
        };
    }

    private static RunComparisonSide MapSideToResponse(ComparisonSideResultDto side)
    {
        var weekItems = new List<RunComparisonWeekItem>();
        for (int i = 0; i < side.WeeklyData.Count; i++)
        {
            var w = side.WeeklyData[i];
            decimal? wow = null;
            if (i > 0)
            {
                var prev = side.WeeklyData[i - 1].SegmentShare;
                if (prev > 0)
                {
                    wow = Math.Round((w.SegmentShare - prev) / prev * 100, 2);
                }
            }
            weekItems.Add(new RunComparisonWeekItem
            {
                WeekStart = w.WeekStart,
                WeekLabel = w.WeekLabel,
                TotalPolicy = w.TotalPolicy,
                SegmentCount = w.SegmentCount,
                SegmentShare = w.SegmentShare,
                WoW = wow
            });
        }

        return new RunComparisonSide
        {
            TotalPolicy = side.TotalPolicy,
            SegmentCount = side.SegmentCount,
            StartShare = side.StartShare,
            EndShare = side.EndShare,
            Change = side.Change,
            GrowthMultiple = side.GrowthMultiple,
            WeeklyData = weekItems
        };
    }
}
using DataAnalysis.Application.Common.Enums;
using DataAnalysis.Application.Features.CustomSegment.Abstractions;
using DataAnalysis.Application.Features.CustomSegment.Dtos;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DataAnalysis.Application.Features.CustomSegment.Commands.RunSegment;

public class RunSegmentHandler : IRequestHandler<RunSegmentCommand, RunSegmentResponse>
{
    private readonly ICustomSegmentRepository _octopusRepository;
    private readonly ICustomSegmentDbRepository _dbRepository;
    private readonly ICustomSegmentAiService _aiService;
    private readonly ILogger<RunSegmentHandler> _logger;

    public RunSegmentHandler(
        ICustomSegmentRepository octopusRepository,
        ICustomSegmentDbRepository dbRepository,
        ICustomSegmentAiService aiService,
        ILogger<RunSegmentHandler> logger)
    {
        _octopusRepository = octopusRepository;
        _dbRepository = dbRepository;
        _aiService = aiService;
        _logger = logger;
    }

    public async Task<RunSegmentResponse> Handle(RunSegmentCommand request, CancellationToken cancellationToken)
    {
        var segment = await _dbRepository.GetByIdAsync(request.SegmentId, cancellationToken);
        if (segment == null)
        {
            return new RunSegmentResponse
            {
                Success = false,
                Message = "Segment bulunamadı."
            };
        }

        var startDate = request.WeekStart.AddDays(-49);
        var endDate = request.WeekEnd;

        // Cache kontrol
        var cachedResult = await _dbRepository.GetCachedResultAsync(
            request.SegmentId, startDate, endDate, cancellationToken);

        if (cachedResult != null)
        {
            return MapToResponse(cachedResult, fromCache: true);
        }

        // Octopus'tan hesapla
        var productGroup = Enum.Parse<ProductGroup>(segment.ProductGroup);
        var weeklyData = await _octopusRepository.CalculateDriftAsync(
            productGroup, segment.Filters, startDate, endDate, cancellationToken);

        if (weeklyData.Count == 0)
        {
            return new RunSegmentResponse
            {
                Success = false,
                Message = "Seçilen kriterlere uygun veri bulunamadı."
            };
        }

        var firstWeek = weeklyData.First();
        var lastWeek = weeklyData.Last();

        var resultDto = new SegmentResultDto
        {
            SegmentId = request.SegmentId,
            StartDate = startDate,
            EndDate = endDate,
            TotalPolicy = weeklyData.Sum(x => x.TotalPolicy),
            SegmentCount = weeklyData.Sum(x => x.SegmentCount),
            StartShare = firstWeek.SegmentShare,
            EndShare = lastWeek.SegmentShare,
            Change = lastWeek.SegmentShare - firstWeek.SegmentShare,
            GrowthMultiple = firstWeek.SegmentShare > 0
                ? Math.Round(lastWeek.SegmentShare / firstWeek.SegmentShare, 2)
                : 0,
            WeeklyData = weeklyData.Select(w => new SegmentDriftWeekDto
            {
                WeekStart = w.WeekStart,
                WeekLabel = w.WeekLabel,
                TotalPolicy = w.TotalPolicy,
                SegmentCount = w.SegmentCount,
                SegmentShare = w.SegmentShare
            }).ToList()
        };

        // Sonucu kaydet
        var savedResult = await _dbRepository.AddResultAsync(request.SegmentId, resultDto, request.UserId, cancellationToken);

        // 3 AI'dan paralel yorum al
        try
        {
            var deepSeekTask = _aiService.GenerateCommentAsync(segment, savedResult, AiModelType.DeepSeek, cancellationToken);
            var geminiTask = _aiService.GenerateCommentAsync(segment, savedResult, AiModelType.Gemini, cancellationToken);
            var gptTask = _aiService.GenerateCommentAsync(segment, savedResult, AiModelType.Gpt, cancellationToken);

            await Task.WhenAll(deepSeekTask, geminiTask, gptTask);

            savedResult.AiCommentDeepSeek = deepSeekTask.Result;
            savedResult.AiCommentGemini = geminiTask.Result;
            savedResult.AiCommentGpt = gptTask.Result;

            // DB'ye kaydet
            await _dbRepository.UpdateResultAiCommentsAsync(
                savedResult.Id,
                savedResult.AiCommentDeepSeek,
                savedResult.AiCommentGemini,
                savedResult.AiCommentGpt,
                cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "RunSegment AI yorumu alınırken hata: SegmentId={SegmentId}, ResultId={ResultId}",
                request.SegmentId, savedResult.Id);
            // AI hatası drift sonucunu engellemez; yorumlar boş kalır, kullanıcı sonuçları görmeye devam eder.
        }

        return MapToResponse(savedResult, fromCache: false);
    }

    private static RunSegmentResponse MapToResponse(SegmentResultDto result, bool fromCache)
    {
        var weekItems = new List<RunSegmentWeekItem>();
        for (int i = 0; i < result.WeeklyData.Count; i++)
        {
            var week = result.WeeklyData[i];
            decimal? wow = null;

            if (i > 0)
            {
                var prevShare = result.WeeklyData[i - 1].SegmentShare;
                if (prevShare > 0)
                {
                    wow = Math.Round((week.SegmentShare - prevShare) / prevShare * 100, 2);
                }
            }

            weekItems.Add(new RunSegmentWeekItem
            {
                WeekStart = week.WeekStart,
                WeekLabel = week.WeekLabel,
                TotalPolicy = week.TotalPolicy,
                SegmentCount = week.SegmentCount,
                SegmentShare = week.SegmentShare,
                WoW = wow
            });
        }

        return new RunSegmentResponse
        {
            ResultId = result.Id,
            TotalPolicy = result.TotalPolicy,
            SegmentCount = result.SegmentCount,
            StartShare = result.StartShare,
            EndShare = result.EndShare,
            Change = result.Change,
            GrowthMultiple = result.GrowthMultiple,
            WeeklyData = weekItems,
            AiCommentDeepSeek = result.AiCommentDeepSeek,
            AiCommentGemini = result.AiCommentGemini,
            AiCommentGpt = result.AiCommentGpt,
            FromCache = fromCache,
            Success = true
        };
    }
}
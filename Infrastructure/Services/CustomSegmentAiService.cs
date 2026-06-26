using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using DataAnalysis.Application.Common.Enums;
using DataAnalysis.Application.Common.Settings;
using DataAnalysis.Application.Features.CustomSegment.Abstractions;
using DataAnalysis.Application.Features.CustomSegment.Dtos;

namespace DataAnalysis.Infrastructure.Services;

public class CustomSegmentAiService : ICustomSegmentAiService
{
    private readonly HttpClient _httpClient;
    private readonly OllamaSettings _settings;
    private readonly ILogger<CustomSegmentAiService> _logger;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public CustomSegmentAiService(
        HttpClient httpClient,
        IOptions<OllamaSettings> settings,
        ILogger<CustomSegmentAiService> logger)
    {
        _httpClient = httpClient;
        _settings = settings.Value;
        _logger = logger;

        _httpClient.BaseAddress = new Uri(_settings.BaseUrl);
        _httpClient.Timeout = TimeSpan.FromSeconds(_settings.TimeoutSeconds);
    }

    public async Task<string> GenerateCommentAsync(
        SegmentDto segment,
        SegmentResultDto result,
        AiModelType modelType,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var prompt = BuildPrompt(segment, result);
            return await CallOllamaAsync(prompt, modelType, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Custom segment AI yorumu oluşturulurken hata: {SegmentId}, Model: {Model}", 
                segment.Id, modelType);
            return "AI yorumu şu anda oluşturulamıyor.";
        }
    }

    public async Task<string> GenerateComparisonCommentAsync(
        SegmentDto segmentA,
        SegmentResultDto resultA,
        SegmentDto segmentB,
        SegmentResultDto resultB,
        AiModelType modelType,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var prompt = BuildComparisonPrompt(segmentA, resultA, segmentB, resultB);
            return await CallOllamaAsync(prompt, modelType, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Segment karşılaştırma AI yorumu oluşturulurken hata: {SegmentA} vs {SegmentB}, Model: {Model}", 
                segmentA.Id, segmentB.Id, modelType);
            return "AI yorumu şu anda oluşturulamıyor.";
        }
    }

    private async Task<string> CallOllamaAsync(string prompt, AiModelType modelType, CancellationToken cancellationToken)
    {
        var modelName = GetModelName(modelType);

        if (string.IsNullOrEmpty(modelName))
        {
            return $"{modelType} modeli yapılandırılmamış.";
        }

        var requestBody = new
        {
            model = modelName,
            prompt = prompt,
            stream = false,
            options = new
            {
                temperature = 0.3,
                num_predict = 1500
            }
        };

        var json = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("/api/generate", content, cancellationToken);
        response.EnsureSuccessStatusCode();

        var responseJson = await response.Content.ReadAsStringAsync(cancellationToken);
        var responseObj = JsonSerializer.Deserialize<OllamaResponse>(responseJson, JsonOptions);

        return CleanResponse(responseObj?.Response ?? "Yorum oluşturulamadı.");
    }

    private string GetModelName(AiModelType modelType)
    {
        return modelType switch
        {
            AiModelType.DeepSeek => _settings.Models?.DeepSeek?.Name ?? "",
            AiModelType.Gemini => _settings.Models?.Gemini?.Name ?? "",
            AiModelType.Gpt => _settings.Models?.Gpt?.Name ?? "",
            _ => ""
        };
    }

    private static string BuildPrompt(SegmentDto segment, SegmentResultDto result)
    {
        var sb = new StringBuilder();

        sb.AppendLine("Sen bir sigorta portföy analistisin. Aşağıdaki özel segment drift analizini yorumla.");
        sb.AppendLine();
        sb.AppendLine("## Segment Tanımı");
        sb.AppendLine($"- Segment Adı: {segment.Name}");
        sb.AppendLine($"- Ürün Grubu: {segment.ProductGroup}");

        AppendSegmentFilters(sb, segment);

        sb.AppendLine();
        sb.AppendLine("## Analiz Sonuçları");
        sb.AppendLine($"- Analiz Dönemi: {result.StartDate:dd.MM.yyyy} - {result.EndDate:dd.MM.yyyy}");
        sb.AppendLine($"- Toplam Poliçe: {result.TotalPolicy:N0}");
        sb.AppendLine($"- Segment Poliçe: {result.SegmentCount:N0}");
        sb.AppendLine($"- Başlangıç Payı: %{result.StartShare:F2}");
        sb.AppendLine($"- Bitiş Payı: %{result.EndShare:F2}");
        sb.AppendLine($"- Değişim: %{result.Change:F2}");
        sb.AppendLine($"- Büyüme Katsayısı: {result.GrowthMultiple:F2}x");

        sb.AppendLine();
        sb.AppendLine("## Haftalık Veriler");
        foreach (var week in result.WeeklyData)
        {
            sb.AppendLine($"- {week.WeekLabel}: Toplam {week.TotalPolicy:N0}, Segment {week.SegmentCount:N0}, Pay %{week.SegmentShare:F2}");
        }

        sb.AppendLine();
        sb.AppendLine("## Talimatlar");
        sb.AppendLine("- Bu segmentin portföy içindeki drift'ini (kayma) değerlendir");
        sb.AppendLine("- Trend yönünü belirle (büyüme/daralma/stabil)");
        sb.AppendLine("- Olası nedenleri ve riskleri kısaca açıkla");
        sb.AppendLine("- Maksimum 3-4 cümle ile özetle");
        sb.AppendLine("- Türkçe yaz, profesyonel bir ton kullan");

        return sb.ToString();
    }

    private static string BuildComparisonPrompt(SegmentDto segmentA, SegmentResultDto resultA, SegmentDto segmentB, SegmentResultDto resultB)
    {
        var sb = new StringBuilder();

        sb.AppendLine("Sen bir sigorta portföy analistisin. Aşağıdaki iki segmenti karşılaştırıp kapsamlı bir değerlendirme yaz.");
        sb.AppendLine();
        
        // Segment A
        sb.AppendLine("## SEGMENT A: " + segmentA.Name);
        AppendSegmentFilters(sb, segmentA);
        sb.AppendLine($"- Başlangıç Payı: %{resultA.StartShare:F2}");
        sb.AppendLine($"- Bitiş Payı: %{resultA.EndShare:F2}");
        sb.AppendLine($"- Değişim: %{resultA.Change:F2}");
        sb.AppendLine($"- Büyüme Katsayısı: {resultA.GrowthMultiple:F2}x");
        sb.AppendLine($"- Segment Poliçe: {resultA.SegmentCount:N0}");
        sb.AppendLine();

        // Segment B
        sb.AppendLine("## SEGMENT B: " + segmentB.Name);
        AppendSegmentFilters(sb, segmentB);
        sb.AppendLine($"- Başlangıç Payı: %{resultB.StartShare:F2}");
        sb.AppendLine($"- Bitiş Payı: %{resultB.EndShare:F2}");
        sb.AppendLine($"- Değişim: %{resultB.Change:F2}");
        sb.AppendLine($"- Büyüme Katsayısı: {resultB.GrowthMultiple:F2}x");
        sb.AppendLine($"- Segment Poliçe: {resultB.SegmentCount:N0}");
        sb.AppendLine();

        sb.AppendLine("## Yorum Yapısı");
        sb.AppendLine("Yorumunu üç bölüm halinde yaz, her bölüm 2-3 cümle olsun:");
        sb.AppendLine();
        sb.AppendLine("**1. Sayısal Karşılaştırma**");
        sb.AppendLine("İki segmentin pay, değişim ve büyüme değerlerini karşılaştır. Hangisinin daha yüksek pay aldığını ve hangisinin daha hızlı büyüdüğünü net sayılarla belirt.");
        sb.AppendLine();
        sb.AppendLine("**2. Davranışsal Analiz**");
        sb.AppendLine("Hangi segmentin trend olarak iyi gittiğini, hangisinin risk taşıdığını yorumla. Değişim yönlerini (düşüş/yükseliş/stabil) değerlendir ve risk açısından karşılaştır.");
        sb.AppendLine();
        sb.AppendLine("**3. Aksiyon Önerisi**");
        sb.AppendLine("Portföy yönetimi açısından somut öneri ver. Örn: hangi segmentin fiyatlandırmasının gözden geçirilmesi, hangi segmente pazarlama odağı verilmesi, hangisinde risk izleme sıklığının artırılması gerektiği gibi pratik aksiyonlar öner.");
        sb.AppendLine();
        sb.AppendLine("Türkçe yaz, profesyonel bir ton kullan. Başlıkları aynen koru (kalın yazılarla).");

        return sb.ToString();
    }

    private static void AppendSegmentFilters(StringBuilder sb, SegmentDto segment)
    {
        if (segment.Filters.Brands?.Any() == true)
            sb.AppendLine($"- Markalar: {string.Join(", ", segment.Filters.Brands)}");
        if (segment.Filters.InsuredAges?.Any() == true)
            sb.AppendLine($"- Sigortalı Yaşları: {string.Join(", ", segment.Filters.InsuredAges)}");
        if (segment.Filters.InsuredTypes?.Any() == true)
            sb.AppendLine($"- Sigortalı Tipleri: {string.Join(", ", segment.Filters.InsuredTypes)}");
        if (segment.Filters.Genders?.Any() == true)
            sb.AppendLine($"- Cinsiyetler: {string.Join(", ", segment.Filters.Genders)}");
        if (segment.Filters.VehicleAges?.Any() == true)
            sb.AppendLine($"- Araç Yaşları: {string.Join(", ", segment.Filters.VehicleAges)}");
        if (segment.Filters.VehicleValues?.Any() == true)
            sb.AppendLine($"- Araç Bedeli: {string.Join(", ", segment.Filters.VehicleValues)}");
    }

    private static string CleanResponse(string response)
    {
        var thinkStart = response.IndexOf("<think>", StringComparison.OrdinalIgnoreCase);
        if (thinkStart >= 0)
        {
            var thinkEnd = response.IndexOf("</think>", StringComparison.OrdinalIgnoreCase);
            if (thinkEnd > thinkStart)
            {
                response = response.Remove(thinkStart, thinkEnd - thinkStart + 8);
            }
        }

        return response.Trim();
    }

    private class OllamaResponse
    {
        public string? Response { get; set; }
    }
}
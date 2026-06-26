using System.Diagnostics;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using DataAnalysis.Application.Common.Enums;
using DataAnalysis.Application.Common.Settings;
using DataAnalysis.Application.Features.ExecSummary.Abstractions;
using DataAnalysis.Application.Features.ExecSummary.Dtos;

namespace DataAnalysis.Infrastructure.Services;

public class OllamaService : IAiSummaryService
{
    private readonly HttpClient _httpClient;
    private readonly OllamaSettings _settings;
    private readonly ILogger<OllamaService> _logger;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public OllamaService(
        HttpClient httpClient,
        IOptions<OllamaSettings> settings,
        ILogger<OllamaService> logger)
    {
        _httpClient = httpClient;
        _settings = settings.Value;
        _logger = logger;

        _httpClient.BaseAddress = new Uri(_settings.BaseUrl);
        _httpClient.Timeout = TimeSpan.FromSeconds(_settings.TimeoutSeconds);
    }

    // Mevcut tek model metodu (geriye uyumluluk)
    public async Task<ExecAiDto> GenerateStructuredSummaryAsync(
        AiSummaryDataDto data,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var prompt = BuildStructuredPrompt(data);
            var response = await GenerateAsync(prompt, GetDefaultModelName(), cancellationToken);
            return ParseStructuredResponse(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Yapılandırılmış AI özet oluşturulurken hata oluştu");
            return CreateFallbackResponse(ex.Message);
        }
    }

    // Yeni: Tek model için (3 ayrı endpoint kullanacak)
    public async Task<ModelResponseDto> GenerateForSingleModelAsync(
        AiSummaryDataDto data,
        AiModelType modelType,
        CancellationToken cancellationToken = default)
    {
        var prompt = BuildStructuredPrompt(data);
        var sw = Stopwatch.StartNew();

        var (modelName, displayName) = modelType switch
        {
            AiModelType.DeepSeek => (_settings.Models?.DeepSeek?.Name ?? "", "DeepSeek"),
            AiModelType.Gemini => (_settings.Models?.Gemini?.Name ?? "", "Gemini"),
            AiModelType.Gpt => (_settings.Models?.Gpt?.Name ?? "", "GPT"),
            _ => ("", "Unknown")
        };

        var response = new ModelResponseDto
        {
            ModelName = displayName,
            ModelDisplayName = displayName
        };

        if (string.IsNullOrEmpty(modelName))
        {
            response.Success = false;
            response.Error = $"{displayName} modeli yapılandırılmamış";
            response.Data = CreateFallbackResponse($"{displayName} modeli yapılandırılmamış");
            return response;
        }

        try
        {
            _logger.LogInformation("Model {ModelName} için istek başlatılıyor", displayName);

            var rawResponse = await GenerateAsync(prompt, modelName, cancellationToken);
            var parsed = ParseStructuredResponse(rawResponse);

            response.Data = parsed;
            response.Success = true;

            _logger.LogInformation("Model {ModelName} başarılı, süre: {Duration}ms", displayName, sw.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Model {ModelName} için hata oluştu", displayName);
            response.Success = false;
            response.Error = ex.Message;
            response.Data = CreateFallbackResponse(ex.Message);
        }
        finally
        {
            sw.Stop();
            response.DurationMs = (int)sw.ElapsedMilliseconds;
        }

        return response;
    }

    private string GetDefaultModelName()
    {
        if (_settings.Models?.DeepSeek?.Enabled == true && !string.IsNullOrEmpty(_settings.Models.DeepSeek.Name))
            return _settings.Models.DeepSeek.Name;
        if (_settings.Models?.Gemini?.Enabled == true && !string.IsNullOrEmpty(_settings.Models.Gemini.Name))
            return _settings.Models.Gemini.Name;
        if (_settings.Models?.Gpt?.Enabled == true && !string.IsNullOrEmpty(_settings.Models.Gpt.Name))
            return _settings.Models.Gpt.Name;

        return _settings.Model ?? string.Empty;
    }

    private async Task<string> GenerateAsync(string prompt, string modelName, CancellationToken cancellationToken)
    {
        var request = new
        {
            model = modelName,
            prompt = prompt,
            stream = false,
            options = new
            {
                temperature = 0.3,
                top_p = 0.9,
                num_predict = 8192  
            }
        };

        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        _logger.LogInformation("Ollama'ya istek gönderiliyor: {Model}", modelName);

        var response = await _httpClient.PostAsync("/api/generate", content, cancellationToken);
        response.EnsureSuccessStatusCode();

        var responseJson = await response.Content.ReadAsStringAsync(cancellationToken);

        _logger.LogInformation("Ollama yanıt uzunluğu: {Length} karakter", responseJson.Length);

        if (string.IsNullOrWhiteSpace(responseJson))
        {
            _logger.LogWarning("Ollama boş yanıt döndü");
            return "AI yanıtı alınamadı - boş yanıt.";
        }

        try
        {
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var result = JsonSerializer.Deserialize<OllamaResponse>(responseJson, options);

            if (result == null || string.IsNullOrWhiteSpace(result.Response))
            {
                _logger.LogWarning("Ollama yanıtı parse edilemedi veya boş");
                return "AI yanıtı parse edilemedi.";
            }

            _logger.LogInformation("Ollama yanıtı başarıyla alındı: {Length} karakter", result.Response.Length);
            return result.Response;
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Ollama JSON parse hatası");
            return "AI yanıtı işlenirken hata oluştu.";
        }
    }

    private string BuildStructuredPrompt(AiSummaryDataDto data)
    {
        var sb = new StringBuilder();

        sb.AppendLine(@"Sen Türkiye'nin önde gelen sigorta şirketlerinden birinde çalışan kıdemli bir aktüerya uzmanısın. 
Portföy risk analizi, fiyatlandırma ve hasar tahminleri konusunda uzmansın.");
        sb.AppendLine();
        sb.AppendLine("Aşağıdaki portföy verilerini analiz et ve SADECE JSON formatında yanıt ver.");
        sb.AppendLine();
        
        AppendPortfolioData(sb, data);
        
        sb.AppendLine();
        sb.AppendLine("SADECE aşağıdaki JSON formatında yanıt ver, başka hiçbir şey yazma:");
        sb.AppendLine(@"
{
  ""generalStatus"": {
    ""summary"": ""2 PARAGRAF DETAYLI ANALİZ: İlk paragraf portföyün genel durumu ve kritik riskleri (drift, basamak 0, yaşlı araç yoğunluğu). İkinci paragraf spesifik segmentler ve trend analizi. Her paragraf 3-4 cümle olmalı."",
    ""metrics"": [
      { ""label"": ""Toplam Poliçe"", ""value"": ""16.312"", ""level"": ""info"" },
      { ""label"": ""Basamak 0"", ""value"": ""%43 (7.048)"", ""level"": ""danger"" },
      { ""label"": ""6-15 Yaş Araç"", ""value"": ""%55"", ""level"": ""warning"" },
      { ""label"": ""Premium Marka"", ""value"": ""2.554"", ""level"": ""danger"" },
      { ""label"": ""Segment Drift"", ""value"": ""8x büyüme"", ""level"": ""danger"" }
    ]
  },
  ""portfolioSummary"": {
    ""characteristics"": [
      ""Orta yaş araç ağırlıklı portföy (3-11 yaş bandı yoğun)"",
      ""Alman marka yoğunluğu belirgin (VW, BMW, Mercedes)"",
      ""Yeni iş akışı güçlü (haftalık 1.500+ poliçe)""
    ],
    ""riskAreas"": [
      ""Basamak 0 oranı %43 ile kritik seviyede"",
      ""Premium marka + yaşlı araç segmenti 8x drift gösteriyor"",
      ""Genç sürücü + premium marka kombinasyonu artış eğiliminde""
    ],
    ""positiveFactors"": [
      ""Haftalık poliçe hacmi istikrarlı artış gösteriyor"",
      ""Marka çeşitliliği risk dağılımını destekliyor"",
      ""3-7 yaş bandı dengeli ve düşük riskli""
    ]
  },
  ""findings"": [
    { ""title"": ""Kısa başlık"", ""description"": ""EN AZ 2-3 CÜMLE DETAYLI AÇIKLAMA. Sayısal veriler, yüzdeler ve karşılaştırmalar içermeli. Riskin ne olduğunu, neden önemli olduğunu ve potansiyel etkisini açıkla."", ""level"": ""critical|high|medium"" }
  ],
  ""recommendations"": [
    { ""title"": ""Öneri başlığı"", ""description"": ""DETAYLI AKSİYON İÇERİKLİ: Ne yapılmalı, nasıl yapılmalı, hangi segmente uygulanmalı. Somut yüzde veya rakam önerisi içermeli."", ""icon"": ""⚡|🛡️|🔧"" }
  ]
}");
        sb.AppendLine();
        sb.AppendLine("KRİTİK KURALLAR:");
        sb.AppendLine("- Sadece JSON döndür, açıklama veya markdown ekleme");
        sb.AppendLine("- generalStatus.summary: TAM 2 PARAGRAF, her paragraf 3-4 cümle, toplam en az 150 kelime");
        sb.AppendLine("- portfolioSummary.characteristics: TAM 3 madde - portföyün genel yapısını tanımla (araç yaşı, marka mixi, iş hacmi)");
        sb.AppendLine("- portfolioSummary.riskAreas: TAM 3 madde - en kritik risk faktörlerini listele (basamak 0, drift, segment riskleri)");
        sb.AppendLine("- portfolioSummary.positiveFactors: TAM 3 madde - olumlu göstergeleri listele (büyüme, çeşitlilik, dengeli segmentler)");
        sb.AppendLine("- findings dizisinde TAM 5 bulgu olsun (3 critical, 2 high)");
        sb.AppendLine("- Her finding.description EN AZ 2-3 cümle detaylı olsun, sayısal veri içersin");
        sb.AppendLine("- recommendations dizisinde TAM 3 öneri olsun");
        sb.AppendLine("- Her recommendation.description somut aksiyon ve yüzde/rakam içersin");
        sb.AppendLine("- metrics dizisinde TAM 5 metrik olsun (verideki gerçek değerlerle)");
        sb.AppendLine("- level değerleri: critical, high, medium (findings için) ve success, warning, danger, info (metrics için)");
        sb.AppendLine("- Tüm metinler Türkçe olmalı, profesyonel aktüeryal dil kullan");
        sb.AppendLine("- Her madde kısa ve öz olsun (maksimum 15 kelime)");

        return sb.ToString();
    }

    private void AppendPortfolioData(StringBuilder sb, AiSummaryDataDto data)
    {
        sb.AppendLine("═══════════════════════════════════════════════════════════════");
        sb.AppendLine("                         PORTFÖY VERİLERİ                        ");
        sb.AppendLine("═══════════════════════════════════════════════════════════════");
        sb.AppendLine();

        // Drift Analizi
        sb.AppendLine("📈 PORTFÖY DRİFT ANALİZİ");
        sb.AppendLine("   Segment 1: Genç Sürücü (18-25) + Premium Marka + Yaşlı Araç (11+ yıl)");
        sb.AppendLine($"      Başlangıç: %{data.Seg1StartShare:F2} → Bitiş: %{data.Seg1EndShare:F2}");
        sb.AppendLine();
        sb.AppendLine("   Segment 2: Tüzel Müşteri + Premium Marka + Yaşlı Araç (11+ yıl)");
        sb.AppendLine($"      Başlangıç: %{data.Seg2StartShare:F2} → Bitiş: %{data.Seg2EndShare:F2}");
        sb.AppendLine();

        if (data.DriftTrend.Any())
        {
            sb.AppendLine("   Haftalık Trend:");
            foreach (var week in data.DriftTrend)
            {
                sb.AppendLine($"      {week.WeekLabel}: {week.TotalPolicy:N0} poliçe | Seg1: %{week.Seg1Share:F2} | Seg2: %{week.Seg2Share:F2}");
            }
            sb.AppendLine();
        }

        // Risk Segmentleri
        if (data.RiskSegments.Any())
        {
            sb.AppendLine("🚨 KRİTİK RİSK SEGMENTLERİ");
            foreach (var seg in data.RiskSegments)
            {
                sb.AppendLine($"   • {seg.Name}: {seg.PolicyCount:N0} poliçe | Seviye: {seg.Severity}");
                sb.AppendLine($"     {seg.Description}");
            }
            sb.AppendLine();
        }

        // Marka Dağılımı
        if (data.BrandDistribution.Any())
        {
            sb.AppendLine("🚗 MARKA DAĞILIMI (Top 10)");
            foreach (var brand in data.BrandDistribution.Take(10))
            {
                sb.AppendLine($"   • {brand.Label}: {brand.Count:N0} poliçe (%{brand.Share:F1})");
            }
            sb.AppendLine();
        }

        // Marka x Yaş Matrisi
        if (data.BrandAgeMatrix.Any())
        {
            sb.AppendLine("📋 MARKA × ARAÇ YAŞI MATRİSİ");
            sb.AppendLine("   Marka          | 0-2   | 3-5   | 6-10  | 11-15 | 16+   | Toplam");
            sb.AppendLine("   ---------------|-------|-------|-------|-------|-------|-------");
            foreach (var row in data.BrandAgeMatrix.Take(10))
            {
                sb.AppendLine($"   {row.Brand,-15}| {row.Age0To2,5} | {row.Age3To5,5} | {row.Age6To10,5} | {row.Age11To15,5} | {row.Age16Plus,5} | {row.Total,5}");
            }
            sb.AppendLine();
        }

        // Basamak Dağılımı
        if (data.StepDistribution.Any())
        {
            sb.AppendLine("📊 HASARSIZLIK BASAMAK DAĞILIMI");
            foreach (var step in data.StepDistribution)
            {
                sb.AppendLine($"   • {step.Label}: {step.Count:N0} poliçe (%{step.Share:F1})");
            }
            sb.AppendLine();
        }

        // Yaş x Basamak Matrisi
        if (data.AgeStepMatrix.Any())
        {
            sb.AppendLine("📋 ARAÇ YAŞI × BASAMAK MATRİSİ");
            sb.AppendLine("   Araç Yaşı | B0    | B1    | B2    | B3    | B4+   | Toplam");
            sb.AppendLine("   ----------|-------|-------|-------|-------|-------|-------");
            foreach (var row in data.AgeStepMatrix)
            {
                sb.AppendLine($"   {row.AgeGroup,-10}| {row.Step0,5} | {row.Step1,5} | {row.Step2,5} | {row.Step3,5} | {row.Step4Plus,5} | {row.Total,5}");
            }
            sb.AppendLine();
        }

        // Sigortalı Yaşı
        if (data.InsuredAgeDistribution.Any())
        {
            sb.AppendLine("👤 SİGORTALI YAŞI DAĞILIMI");
            foreach (var age in data.InsuredAgeDistribution)
            {
                sb.AppendLine($"   • {age.Label}: {age.Count:N0} poliçe (%{age.Share:F1})");
            }
            sb.AppendLine();
        }

        // Genç Sürücü x Marka
        if (data.YoungDriverDistribution.Any())
        {
            sb.AppendLine("🚨 GENÇ SÜRÜCÜ (18-25) × MARKA");
            foreach (var item in data.YoungDriverDistribution)
            {
                sb.AppendLine($"   • {item.Label}: {item.Count:N0} genç sürücü");
            }
            sb.AppendLine();
        }

        // Araç Yaşı Dağılımı
        if (data.VehicleAgeDistribution.Any())
        {
            sb.AppendLine("🚙 ARAÇ YAŞI DAĞILIMI");
            foreach (var age in data.VehicleAgeDistribution.Take(10))
            {
                sb.AppendLine($"   • {age.Label} yaş: {age.Count:N0} poliçe (%{age.Share:F1})");
            }
            sb.AppendLine();
        }

        sb.AppendLine("═══════════════════════════════════════════════════════════════");
    }

    private ExecAiDto ParseStructuredResponse(string response)
    {
        try
        {
            var jsonContent = ExtractJsonFromResponse(response);
            
            if (string.IsNullOrWhiteSpace(jsonContent))
            {
                _logger.LogWarning("JSON içeriği bulunamadı, fallback kullanılıyor");
                return CreateFallbackResponse("JSON içeriği bulunamadı");
            }

            var result = JsonSerializer.Deserialize<ExecAiDto>(jsonContent, JsonOptions);
            
            if (result == null)
            {
                return CreateFallbackResponse("JSON parse edilemedi");
            }

            result.CriticalCount = result.Findings.Count(f => f.Level == "critical");
            result.HighCount = result.Findings.Count(f => f.Level == "high");
            result.MediumCount = result.Findings.Count(f => f.Level == "medium");
            result.CreatedAt = DateTime.UtcNow;

            _logger.LogInformation("Yapılandırılmış özet başarıyla parse edildi: {FindingCount} bulgu, {RecommendationCount} öneri",
                result.Findings.Count, result.Recommendations.Count);

            return result;
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Yapılandırılmış yanıt JSON parse hatası: {Response}", response);
            return CreateFallbackResponse($"JSON parse hatası: {ex.Message}");
        }
    }

    private string ExtractJsonFromResponse(string response)
    {
        if (string.IsNullOrWhiteSpace(response))
            return string.Empty;

        var jsonStartMarker = "```json";
        var jsonEndMarker = "```";
        
        var startIndex = response.IndexOf(jsonStartMarker, StringComparison.OrdinalIgnoreCase);
        if (startIndex >= 0)
        {
            startIndex += jsonStartMarker.Length;
            var endIndex = response.IndexOf(jsonEndMarker, startIndex, StringComparison.Ordinal);
            if (endIndex > startIndex)
            {
                var extracted = response.Substring(startIndex, endIndex - startIndex).Trim();
                return TryRepairJson(extracted);
            }
            else
            {
                var jsonPart = response.Substring(startIndex).Trim();
                return TryRepairJson(jsonPart);
            }
        }

        var tripleBacktick = "```";
        startIndex = response.IndexOf(tripleBacktick, StringComparison.Ordinal);
        if (startIndex >= 0)
        {
            startIndex += tripleBacktick.Length;
            while (startIndex < response.Length && (response[startIndex] == '\r' || response[startIndex] == '\n'))
                startIndex++;
            
            var endIndex = response.IndexOf(tripleBacktick, startIndex, StringComparison.Ordinal);
            if (endIndex > startIndex)
            {
                var extracted = response.Substring(startIndex, endIndex - startIndex).Trim();
                return TryRepairJson(extracted);
            }
            else
            {
                var jsonPart = response.Substring(startIndex).Trim();
                return TryRepairJson(jsonPart);
            }
        }

        startIndex = response.IndexOf('{');
        if (startIndex >= 0)
        {
            var endIndex = response.LastIndexOf('}');
            if (endIndex > startIndex)
            {
                return response.Substring(startIndex, endIndex - startIndex + 1);
            }
            else
            {
                var jsonPart = response.Substring(startIndex);
                return TryRepairJson(jsonPart);
            }
        }

        return response.Trim();
    }

    private string TryRepairJson(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return string.Empty;

        var startIndex = json.IndexOf('{');
        if (startIndex < 0)
            return string.Empty;
        
        json = json.Substring(startIndex);

        if (IsValidJson(json))
            return json;

        int braceCount = 0;
        int bracketCount = 0;
        bool inString = false;
        char prevChar = '\0';

        foreach (var c in json)
        {
            if (c == '"' && prevChar != '\\')
                inString = !inString;
            
            if (!inString)
            {
                if (c == '{') braceCount++;
                else if (c == '}') braceCount--;
                else if (c == '[') bracketCount++;
                else if (c == ']') bracketCount--;
            }
            prevChar = c;
        }

        if (inString)
            json += "\"";
        var sb = new StringBuilder(json.TrimEnd());
        
        var trimmed = sb.ToString().TrimEnd();
        if (trimmed.EndsWith(","))
            sb = new StringBuilder(trimmed.Substring(0, trimmed.Length - 1));
        
        for (int i = 0; i < bracketCount; i++)
            sb.Append(']');
        for (int i = 0; i < braceCount; i++)
            sb.Append('}');

        var repaired = sb.ToString();

        if (IsValidJson(repaired))
        {
            _logger.LogInformation("Kesik JSON onarıldı, {BraceCount} brace ve {BracketCount} bracket eklendi", braceCount, bracketCount);
            return repaired;
        }

        _logger.LogWarning("JSON onarılamadı, orijinal döndürülüyor");
        return json;
    }

    private bool IsValidJson(string json)
    {
        try
        {
            using var doc = JsonDocument.Parse(json);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private ExecAiDto CreateFallbackResponse(string errorMessage)
    {
        return new ExecAiDto
        {
            GeneralStatus = new GeneralStatusDto
            {
                Summary = "AI özeti şu anda kullanılamıyor. Lütfen daha sonra tekrar deneyin.",
                Metrics = new List<MetricDto>
                {
                    new() { Label = "Durum", Value = "Geçici Hata", Level = "warning" }
                }
            },
            PortfolioSummary = new PortfolioSummaryDto
            {
                Characteristics = new List<string> { "Analiz yapılıyor..." },
                RiskAreas = new List<string>(),
                PositiveFactors = new List<string>()
            },
            Findings = new List<FindingDto>
            {
                new()
                {
                    Title = "AI Servisi Geçici Olarak Kullanılamıyor",
                    Description = $"Teknik detay: {errorMessage}",
                    Level = "medium"
                }
            },
            Recommendations = new List<RecommendationDto>
            {
                new()
                {
                    Title = "Yeniden Deneyin",
                    Description = "Birkaç dakika sonra 'Yenile' butonuna tıklayarak tekrar deneyebilirsiniz.",
                    Icon = "🔄"
                }
            },
            CriticalCount = 0,
            HighCount = 0,
            MediumCount = 1,
            CreatedAt = DateTime.UtcNow
        };
    }

    private class OllamaResponse
    {
        public string Response { get; set; } = string.Empty;
        public bool Done { get; set; }
    }
}
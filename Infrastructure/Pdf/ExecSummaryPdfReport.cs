using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using DataAnalysis.Application.Features.ExecSummary.Dtos;
using DataAnalysis.Application.Features.ExecSummary.Queries.GetAgeStep;
using DataAnalysis.Application.Features.ExecSummary.Queries.GetBrandAge;
using DataAnalysis.Application.Features.ExecSummary.Queries.GetDistribution;
using DataAnalysis.Application.Features.ExecSummary.Queries.GetDrift;
using DataAnalysis.Application.Features.ExecSummary.Queries.GetRisk;
using DataAnalysis.Application.Features.ExecSummary.Queries.GetYoungDriver;

namespace DataAnalysis.Infrastructure.Pdf;

public class ExecSummaryPdfReport : BasePdfReport
{
    private readonly GetDriftQueryResponse _drift;
    private readonly GetBrandAgeQueryResponse _brandAge;
    private readonly GetAgeStepQueryResponse _ageStep;
    private readonly GetYoungDriverQueryResponse _youngDriver;
    private readonly GetRiskQueryResponse _risk;
    private readonly GetDistributionQueryResponse _distribution;
    private readonly ExecAiDto? _deepSeekData;
    private readonly ExecAiDto? _geminiData;
    private readonly ExecAiDto? _gptData;

    private static readonly string[] PremiumBrands = { "BMW", "MERCEDES", "AUDI", "PORSCHE", "LAND ROVER", "JAGUAR", "VOLVO", "LEXUS" };

    // Mor tema renkleri
    private const string PurpleDark = "#7c3aed";
    private const string PurpleMedium = "#8b5cf6";
    private const string PurpleLight = "#a78bfa";
    private const string PurpleBg = "#f5f3ff";
    private const string PurpleBorder = "#c4b5fd";
    private const string PurpleText = "#4c1d95";

    public ExecSummaryPdfReport(
        string productGroup,
        string dateRange,
        GetDriftQueryResponse drift,
        GetBrandAgeQueryResponse brandAge,
        GetAgeStepQueryResponse ageStep,
        GetYoungDriverQueryResponse youngDriver,
        GetRiskQueryResponse risk,
        GetDistributionQueryResponse distribution,
        ExecAiDto? deepSeekData,
        ExecAiDto? geminiData,
        ExecAiDto? gptData)
        : base("Yönetici Özeti", productGroup, dateRange)
    {
        _drift = drift;
        _brandAge = brandAge;
        _ageStep = ageStep;
        _youngDriver = youngDriver;
        _risk = risk;
        _distribution = distribution;
        _deepSeekData = deepSeekData;
        _geminiData = geminiData;
        _gptData = gptData;
    }

    protected override void ComposeContent(IContainer container)
    {
        container.Column(col =>
        {
            col.Spacing(10);

            // Sayfa 1-3: AI Modelleri (Her model kendi sayfasında, tüm verilerle)
            col.Item().Element(c => SectionLabel(c, "YAPAY ZEKA DEĞERLENDİRMESİ"));
            
            // DeepSeek - Tam sayfa
            col.Item().Element(c => ComposeFullModelSection(c, "DeepSeek", _deepSeekData));
            
            // Gemini - Yeni sayfa
            col.Item().PageBreak();
            col.Item().Element(c => ComposeFullModelSection(c, "Gemini", _geminiData));
            
            // GPT - Yeni sayfa
            col.Item().PageBreak();
            col.Item().Element(c => ComposeFullModelSection(c, "GPT", _gptData));

            // Sayfa 4: Drift Analizi + Araç Yaşı × Basamak
            col.Item().PageBreak();
            col.Item().Element(ComposeDriftSection);
            col.Item().PaddingTop(16).Element(ComposeAgeStepMatrix);

            // Sayfa 5: Marka × Araç Yaşı Risk Matrisi
            col.Item().PageBreak();
            col.Item().Element(ComposeBrandAgeMatrix);

            // Sayfa 6: Dağılımlar
            col.Item().PageBreak();
            col.Item().Element(ComposeDistributions);

            // Sayfa 7: Riskli Segmentler + Genç Sürücü
            col.Item().PageBreak();
            col.Item().Element(ComposeRiskSegments);
            col.Item().Element(ComposeYoungDriverSection);
        });
    }

    // ===== TAM MODEL BÖLÜMÜ (Tüm veriler) =====
    private void ComposeFullModelSection(IContainer container, string modelName, ExecAiDto? data)
    {
        container.Background(PurpleBg).Border(1).BorderColor(PurpleBorder).Padding(16).Column(col =>
        {
            // Model Başlığı
            col.Item().Background(PurpleDark).Padding(10).AlignCenter()
                .Text(modelName).FontSize(14).Bold().FontColor("#ffffff");

            if (data == null || data.GeneralStatus == null)
            {
                col.Item().PaddingTop(20).AlignCenter()
                    .Text("Bu model için veri bulunamadı").FontSize(10).FontColor("#94a3b8");
                return;
            }

            // 1. GENEL DURUM
            col.Item().PaddingTop(12).Element(c => ComposeModelGeneralStatus(c, data));

            // 2. KRİTİK RİSK BULGULARI
            if (data.Findings?.Any() == true)
            {
                col.Item().PaddingTop(16).Element(c => ComposeModelFindings(c, data.Findings));
            }

            // 3. KISA VADELİ ÖNERİLER
            if (data.Recommendations?.Any() == true)
            {
                col.Item().PaddingTop(16).Element(c => ComposeModelRecommendations(c, data.Recommendations));
            }

            // 4. PORTFÖY ÖZETİ
            if (data.PortfolioSummary != null)
            {
                col.Item().PaddingTop(16).Element(c => ComposeModelPortfolioSummary(c, data.PortfolioSummary));
            }
        });
    }

    private void ComposeModelGeneralStatus(IContainer container, ExecAiDto data)
    {
        container.Column(col =>
        {
            // Başlık
            col.Item().Text("GENEL DURUM").FontSize(10).Bold().FontColor(PurpleText);
            col.Item().PaddingTop(2).LineHorizontal(1).LineColor(PurpleBorder);

            // Özet metin - TAM METİN
            if (!string.IsNullOrEmpty(data.GeneralStatus.Summary))
            {
                col.Item().PaddingTop(8).Background("#ffffff").Border(0.5f).BorderColor(PurpleBorder).Padding(12)
                    .Text(data.GeneralStatus.Summary).FontSize(9).FontColor("#374151").LineHeight(1.5f);
            }

            // Metrikler
            if (data.GeneralStatus.Metrics?.Any() == true)
            {
                col.Item().PaddingTop(10).Row(row =>
                {
                    foreach (var metric in data.GeneralStatus.Metrics)
                    {
                        var (bg, border, text) = GetLevelColors(metric.Level);
                        row.RelativeItem().Background(bg).Border(0.5f).BorderColor(border).Padding(8).Column(c =>
                        {
                            c.Item().AlignCenter().Text(metric.Value).FontSize(11).Bold().FontColor(text);
                            c.Item().AlignCenter().PaddingTop(3).Text(metric.Label).FontSize(7).FontColor("#64748b");
                        });
                        row.ConstantItem(6);
                    }
                });
            }
        });
    }

    private void ComposeModelFindings(IContainer container, List<FindingDto> findings)
    {
        container.Column(col =>
        {
            // Başlık
            col.Item().Text("KRİTİK RİSK BULGULARI").FontSize(10).Bold().FontColor(PurpleText);
            col.Item().PaddingTop(2).LineHorizontal(1).LineColor(PurpleBorder);

            // Tüm bulgular
            foreach (var finding in findings)
            {
                var (bg, border, _) = GetFindingColors(finding.Level);
                var icon = finding.Level == "critical" ? "🔴" : finding.Level == "high" ? "🟠" : "🟡";

                col.Item().PaddingTop(8).Background(bg).BorderLeft(3).BorderColor(border).Padding(10).Column(c =>
                {
                    c.Item().Text($"{icon} {finding.Title}").FontSize(9).Bold().FontColor(border);
                    c.Item().PaddingTop(4).Text(finding.Description).FontSize(8).FontColor("#475569").LineHeight(1.4f);
                });
            }
        });
    }

    private void ComposeModelRecommendations(IContainer container, List<RecommendationDto> recommendations)
    {
        container.Column(col =>
        {
            // Başlık
            col.Item().Text("KISA VADELİ ÖNERİLER").FontSize(10).Bold().FontColor(PurpleText);
            col.Item().PaddingTop(2).LineHorizontal(1).LineColor(PurpleBorder);

            // Tüm öneriler
            foreach (var rec in recommendations)
            {
                col.Item().PaddingTop(8).Background("#f0fdf4").BorderLeft(3).BorderColor("#10b981").Padding(10).Column(c =>
                {
                    c.Item().Text($"{rec.Icon} {rec.Title}").FontSize(9).Bold().FontColor("#065f46");
                    c.Item().PaddingTop(4).Text(rec.Description).FontSize(8).FontColor("#475569").LineHeight(1.4f);
                });
            }
        });
    }

    private void ComposeModelPortfolioSummary(IContainer container, PortfolioSummaryDto summary)
    {
        container.Column(col =>
        {
            // Başlık
            col.Item().Text("PORTFÖY ÖZETİ").FontSize(10).Bold().FontColor(PurpleText);
            col.Item().PaddingTop(2).LineHorizontal(1).LineColor(PurpleBorder);

            col.Item().PaddingTop(8).Row(row =>
            {
                // Portföy Karakteri
                row.RelativeItem().Background("#f5f3ff").Border(0.5f).BorderColor("#c4b5fd").Padding(10).Column(c =>
                {
                    c.Item().Text("🔍 Portföy Karakteri").FontSize(8).Bold().FontColor("#6d28d9");
                    foreach (var item in summary.Characteristics)
                    {
                        c.Item().PaddingTop(4).Row(r =>
                        {
                            r.ConstantItem(8).Text("•").FontSize(7).FontColor("#8b5cf6");
                            r.RelativeItem().Text(item).FontSize(7).FontColor("#475569").LineHeight(1.3f);
                        });
                    }
                });

                row.ConstantItem(8);

                // Risk Alanları
                row.RelativeItem().Background("#fef2f2").Border(0.5f).BorderColor("#fecaca").Padding(10).Column(c =>
                {
                    c.Item().Text("⚠️ Risk Alanları").FontSize(8).Bold().FontColor("#991b1b");
                    foreach (var item in summary.RiskAreas)
                    {
                        c.Item().PaddingTop(4).Row(r =>
                        {
                            r.ConstantItem(8).Text("•").FontSize(7).FontColor("#dc2626");
                            r.RelativeItem().Text(item).FontSize(7).FontColor("#7f1d1d").LineHeight(1.3f);
                        });
                    }
                });

                row.ConstantItem(8);

                // Olumlu Faktörler
                row.RelativeItem().Background("#f0fdf4").Border(0.5f).BorderColor("#bbf7d0").Padding(10).Column(c =>
                {
                    c.Item().Text("✅ Olumlu Faktörler").FontSize(8).Bold().FontColor("#166534");
                    foreach (var item in summary.PositiveFactors)
                    {
                        c.Item().PaddingTop(4).Row(r =>
                        {
                            r.ConstantItem(8).Text("•").FontSize(7).FontColor("#16a34a");
                            r.RelativeItem().Text(item).FontSize(7).FontColor("#14532d").LineHeight(1.3f);
                        });
                    }
                });
            });
        });
    }

    // ===== DİĞER BÖLÜMLER (Mevcut kodlar) =====

    private void ComposeDriftSection(IContainer container)
    {
        container.Column(col =>
        {
            col.Item().Element(c => SectionLabel(c, "PORTFÖY DRİFT ANALİZİ"));

            // Segment tanımları
            col.Item().Row(row =>
            {
                row.RelativeItem().Background("#fef2f2").Border(0.5f).BorderColor("#fecaca").Padding(10).Column(c =>
                {
                    c.Item().Text("Segment 1 - Riskli Bireysel").FontSize(9).Bold().FontColor("#991b1b");
                    c.Item().PaddingTop(4).Text("Genç sürücü (18-25) + Premium marka + 11+ yaş araç").FontSize(8).FontColor("#dc2626");
                });
                row.ConstantItem(12);
                row.RelativeItem().Background("#fff7ed").Border(0.5f).BorderColor("#fed7aa").Padding(10).Column(c =>
                {
                    c.Item().Text("Segment 2 - Riskli Tüzel").FontSize(9).Bold().FontColor("#9a3412");
                    c.Item().PaddingTop(4).Text("Tüzel müşteri + Premium marka + 11+ yaş araç").FontSize(8).FontColor("#ea580c");
                });
            });

            // Haftalık drift tablosu
            col.Item().PaddingTop(12).Element(ComposeDriftTable);

            // Özet kartlar
            col.Item().PaddingTop(12).Row(row =>
            {
                row.RelativeItem().Background("#fef2f2").Border(0.5f).BorderColor("#fecaca").Padding(12).Column(c =>
                {
                    c.Item().Text("Segment 1 Değişimi").FontSize(8).Bold().FontColor("#991b1b");
                    c.Item().PaddingTop(6).Text($"%{_drift.Seg1StartShare:F2} → %{_drift.Seg1EndShare:F2}").FontSize(11).Bold().FontColor("#dc2626");
                    c.Item().PaddingTop(4).Text($"{_drift.Seg1GrowthMultiple:F1}x büyüme").FontSize(9).FontColor("#991b1b");
                });
                row.ConstantItem(12);
                row.RelativeItem().Background("#fff7ed").Border(0.5f).BorderColor("#fed7aa").Padding(12).Column(c =>
                {
                    c.Item().Text("Segment 2 Değişimi").FontSize(8).Bold().FontColor("#9a3412");
                    c.Item().PaddingTop(6).Text($"%{_drift.Seg2StartShare:F2} → %{_drift.Seg2EndShare:F2}").FontSize(11).Bold().FontColor("#ea580c");
                    c.Item().PaddingTop(4).Text($"{_drift.Seg2GrowthMultiple:F1}x büyüme").FontSize(9).FontColor("#9a3412");
                });
            });
        });
    }

    private void ComposeDriftTable(IContainer container)
    {
        var weeks = _drift.WeeklyTrend;
        if (!weeks.Any()) return;

        container.Table(table =>
        {
            table.ColumnsDefinition(cols =>
            {
                cols.RelativeColumn(2f);
                cols.RelativeColumn();
                cols.RelativeColumn();
                cols.RelativeColumn();
                cols.RelativeColumn();
                cols.RelativeColumn();
            });

            table.Header(header =>
            {
                header.Cell().Background("#0f172a").Padding(6).Text("Hafta").FontSize(8).Bold().FontColor("#ffffff");
                header.Cell().Background("#0f172a").Padding(6).AlignRight().Text("Toplam").FontSize(8).Bold().FontColor("#ffffff");
                header.Cell().Background("#0f172a").Padding(6).AlignRight().Text("Seg1 Adet").FontSize(8).Bold().FontColor("#ffffff");
                header.Cell().Background("#0f172a").Padding(6).AlignRight().Text("Seg1 Pay").FontSize(8).Bold().FontColor("#ffffff");
                header.Cell().Background("#0f172a").Padding(6).AlignRight().Text("Seg2 Adet").FontSize(8).Bold().FontColor("#ffffff");
                header.Cell().Background("#0f172a").Padding(6).AlignRight().Text("Seg2 Pay").FontSize(8).Bold().FontColor("#ffffff");
            });

            foreach (var (week, i) in weeks.Select((w, idx) => (w, idx)))
            {
                var bg = i % 2 == 0 ? "#ffffff" : "#f8fafc";
                var isLast = i == weeks.Count - 1;
                var rowBg = isLast ? "#fef3c7" : bg;

                table.Cell().Background(rowBg).Padding(5).Text(week.WeekLabel).FontSize(8).FontColor(isLast ? "#92400e" : "#1e293b");
                table.Cell().Background(rowBg).Padding(5).AlignRight().Text($"{week.TotalPolicy:N0}").FontSize(8);
                table.Cell().Background(rowBg).Padding(5).AlignRight().Text($"{week.Seg1Count:N0}").FontSize(8);
                
                if (isLast)
                    table.Cell().Background(rowBg).Padding(5).AlignRight().Text($"%{week.Seg1Share:F2}").FontSize(8).FontColor("#dc2626").Bold();
                else
                    table.Cell().Background(rowBg).Padding(5).AlignRight().Text($"%{week.Seg1Share:F2}").FontSize(8).FontColor("#dc2626");
                
                table.Cell().Background(rowBg).Padding(5).AlignRight().Text($"{week.Seg2Count:N0}").FontSize(8);
                
                if (isLast)
                    table.Cell().Background(rowBg).Padding(5).AlignRight().Text($"%{week.Seg2Share:F2}").FontSize(8).FontColor("#ea580c").Bold();
                else
                    table.Cell().Background(rowBg).Padding(5).AlignRight().Text($"%{week.Seg2Share:F2}").FontSize(8).FontColor("#ea580c");
            }
        });
    }

    private void ComposeBrandAgeMatrix(IContainer container)
    {
        container.Column(col =>
        {
            col.Item().Element(c => SectionLabel(c, "MARKA × ARAÇ YAŞI RİSK MATRİSİ"));

            var matrix = _brandAge.Matrix;
            if (!matrix.Any()) return;

            // Top 10 Marka Bar Chart
            col.Item().Text("Top 10 Marka Dağılımı").FontSize(9).Bold().FontColor("#334155");
            col.Item().PaddingTop(6).Element(ct => ComposeBrandBars(ct, matrix));

            // Heatmap Tablosu
            col.Item().PaddingTop(14).Text("Marka × Araç Yaşı Matrisi").FontSize(9).Bold().FontColor("#334155");
            col.Item().PaddingTop(6).Table(table =>
            {
                table.ColumnsDefinition(cols =>
                {
                    cols.RelativeColumn(1.8f);
                    cols.RelativeColumn();
                    cols.RelativeColumn();
                    cols.RelativeColumn();
                    cols.RelativeColumn();
                    cols.RelativeColumn();
                    cols.RelativeColumn();
                });

                table.Header(header =>
                {
                    header.Cell().Background("#0f172a").Padding(4).Text("Marka").FontSize(7).Bold().FontColor("#ffffff");
                    header.Cell().Background("#0f172a").Padding(4).AlignCenter().Text("0-2").FontSize(7).Bold().FontColor("#ffffff");
                    header.Cell().Background("#0f172a").Padding(4).AlignCenter().Text("3-5").FontSize(7).Bold().FontColor("#ffffff");
                    header.Cell().Background("#0f172a").Padding(4).AlignCenter().Text("6-10").FontSize(7).Bold().FontColor("#ffffff");
                    header.Cell().Background("#dc2626").Padding(4).AlignCenter().Text("11-15").FontSize(7).Bold().FontColor("#ffffff");
                    header.Cell().Background("#0f172a").Padding(4).AlignCenter().Text("16+").FontSize(7).Bold().FontColor("#ffffff");
                    header.Cell().Background("#0f172a").Padding(4).AlignCenter().Text("Toplam").FontSize(7).Bold().FontColor("#ffffff");
                });

                var allVals = matrix.SelectMany(m => new[] { m.Age0To2, m.Age3To5, m.Age6To10, m.Age11To15, m.Age16Plus }).Where(v => v > 0).ToList();
                var minVal = allVals.Any() ? allVals.Min() : 0;
                var maxVal = allVals.Any() ? allVals.Max() : 1;

                foreach (var (row, i) in matrix.Select((r, idx) => (r, idx)))
                {
                    var bg = i % 2 == 0 ? "#ffffff" : "#f8fafc";
                    var isPrem = IsPremiumBrand(row.Brand);

                    if (isPrem)
                        table.Cell().Background("#fef2f2").Padding(4).Text(row.Brand).FontSize(7).Bold().FontColor("#dc2626");
                    else
                        table.Cell().Background(bg).Padding(4).Text(row.Brand).FontSize(7).FontColor("#1e293b");
                    
                    table.Cell().Background(bg).Padding(4).AlignCenter().Text($"{row.Age0To2:N0}").FontSize(7);
                    table.Cell().Background(bg).Padding(4).AlignCenter().Text($"{row.Age3To5:N0}").FontSize(7);
                    table.Cell().Background(GetHeatmapBg(row.Age6To10, minVal, maxVal)).Padding(4).AlignCenter().Text($"{row.Age6To10:N0}").FontSize(7);
                    table.Cell().Background(GetHeatmapBg(row.Age11To15, minVal, maxVal)).Padding(4).AlignCenter().Text($"{row.Age11To15:N0}").FontSize(7).Bold();
                    table.Cell().Background(bg).Padding(4).AlignCenter().Text($"{row.Age16Plus:N0}").FontSize(7);
                    table.Cell().Background(bg).Padding(4).AlignCenter().Text($"{row.Total:N0}").FontSize(7).Bold();
                }
            });
        });
    }

    private void ComposeBrandBars(IContainer container, List<BrandAgeMatrixDto> matrix)
    {
        var maxCount = matrix.Max(x => x.Total);
        var totalAll = matrix.Sum(x => x.Total);

        container.Table(table =>
        {
            table.ColumnsDefinition(cols =>
            {
                cols.ConstantColumn(75);
                cols.RelativeColumn();
                cols.ConstantColumn(50);
                cols.ConstantColumn(38);
            });

            foreach (var item in matrix.Take(10))
            {
                var ratio = maxCount > 0 ? (decimal)item.Total / maxCount : 0;
                var barW = (int)(ratio * 100);
                var pct = totalAll > 0 ? (decimal)item.Total / totalAll * 100 : 0;
                var isPrem = IsPremiumBrand(item.Brand);

                if (isPrem)
                    table.Cell().PaddingVertical(2).Text(item.Brand).FontSize(7).FontColor("#dc2626").Bold();
                else
                    table.Cell().PaddingVertical(2).Text(item.Brand).FontSize(7).FontColor("#475569");
                
                table.Cell().PaddingVertical(2).AlignLeft().Row(r =>
                {
                    r.ConstantItem(barW).Height(10).Background(isPrem ? "#ef4444" : "#10b981");
                    r.RelativeItem();
                });
                table.Cell().PaddingVertical(2).AlignRight().Text($"{item.Total:N0}").FontSize(7).FontColor("#1e293b");
                table.Cell().PaddingVertical(2).AlignRight().Text($"%{pct:F1}").FontSize(7).FontColor("#64748b");
            }
        });
    }

    private void ComposeAgeStepMatrix(IContainer container)
    {
        container.Column(col =>
        {
            col.Item().Element(c => SectionLabel(c, "ARAÇ YAŞI × HASARSIZLIK BASAMAK MATRİSİ"));

            var matrix = _ageStep.Matrix;
            if (!matrix.Any()) return;

            col.Item().Table(table =>
            {
                table.ColumnsDefinition(cols =>
                {
                    cols.RelativeColumn(1.5f);
                    cols.RelativeColumn();
                    cols.RelativeColumn();
                    cols.RelativeColumn();
                    cols.RelativeColumn();
                    cols.RelativeColumn();
                    cols.RelativeColumn();
                });

                table.Header(header =>
                {
                    header.Cell().Background("#0f172a").Padding(5).Text("Araç Yaşı").FontSize(7).Bold().FontColor("#ffffff");
                    header.Cell().Background("#dc2626").Padding(5).AlignCenter().Text("B0").FontSize(7).Bold().FontColor("#ffffff");
                    header.Cell().Background("#0f172a").Padding(5).AlignCenter().Text("B1").FontSize(7).Bold().FontColor("#ffffff");
                    header.Cell().Background("#0f172a").Padding(5).AlignCenter().Text("B2").FontSize(7).Bold().FontColor("#ffffff");
                    header.Cell().Background("#0f172a").Padding(5).AlignCenter().Text("B3").FontSize(7).Bold().FontColor("#ffffff");
                    header.Cell().Background("#0f172a").Padding(5).AlignCenter().Text("B4+").FontSize(7).Bold().FontColor("#ffffff");
                    header.Cell().Background("#0f172a").Padding(5).AlignCenter().Text("Toplam").FontSize(7).Bold().FontColor("#ffffff");
                });

                foreach (var (row, i) in matrix.Select((r, idx) => (r, idx)))
                {
                    var bg = i % 2 == 0 ? "#ffffff" : "#f8fafc";
                    var isHighRisk = row.AgeGroup.Contains("6-10") || row.AgeGroup.Contains("11-15");
                    var hlBg = isHighRisk ? "#fef2f2" : bg;

                    table.Cell().Background(hlBg).Padding(5).Text(row.AgeGroup).FontSize(8).Bold().FontColor(isHighRisk ? "#991b1b" : "#1e293b");
                    table.Cell().Background(hlBg).Padding(5).AlignCenter().Text($"{row.Step0:N0}").FontSize(8).FontColor("#dc2626").Bold();
                    table.Cell().Background(bg).Padding(5).AlignCenter().Text($"{row.Step1:N0}").FontSize(8);
                    table.Cell().Background(bg).Padding(5).AlignCenter().Text($"{row.Step2:N0}").FontSize(8);
                    table.Cell().Background(bg).Padding(5).AlignCenter().Text($"{row.Step3:N0}").FontSize(8);
                    table.Cell().Background(bg).Padding(5).AlignCenter().Text($"{row.Step4Plus:N0}").FontSize(8);
                    table.Cell().Background(bg).Padding(5).AlignCenter().Text($"{row.Total:N0}").FontSize(8).Bold();
                }
            });

            var totalStep0 = matrix.Sum(m => m.Step0);
            var totalAll = matrix.Sum(m => m.Total);
            var step0Pct = totalAll > 0 ? (decimal)totalStep0 / totalAll * 100 : 0;

            if (step0Pct > 40)
            {
                col.Item().PaddingTop(12).Background("#fef2f2").Border(0.5f).BorderColor("#fecaca").Padding(10).Text(
                    $"⚠️ Basamak 0 oranı %{step0Pct:F1} ile kritik seviyede."
                ).FontSize(8).FontColor("#991b1b");
            }
        });
    }

    private void ComposeDistributions(IContainer container)
    {
        container.Column(col =>
        {
            col.Item().Element(c => SectionLabel(c, "PORTFÖY DAĞILIMLARI"));

            col.Item().Row(row =>
            {
                row.RelativeItem().Column(c =>
                {
                    c.Item().Text("Marka Dağılımı (Top 10)").FontSize(9).Bold().FontColor("#334155");
                    c.Item().PaddingTop(6).Element(ct => ComposeDistBars(ct, _distribution.Brands, "#3b82f6"));
                });

                row.ConstantItem(20);

                row.RelativeItem().Column(c =>
                {
                    c.Item().Text("Hasarsızlık Basamak Dağılımı").FontSize(9).Bold().FontColor("#334155");
                    c.Item().PaddingTop(6).Element(ct => ComposeStepDist(ct, _distribution.Steps));
                });
            });

            col.Item().PaddingTop(16).Row(row =>
            {
                row.RelativeItem().Column(c =>
                {
                    c.Item().Text("Araç Yaşı Dağılımı").FontSize(9).Bold().FontColor("#334155");
                    c.Item().PaddingTop(6).Element(ct => ComposeVehicleAgeDist(ct, _distribution.VehicleAges));
                });

                row.ConstantItem(20);

                row.RelativeItem().Column(c =>
                {
                    c.Item().Text("Sigortalı Yaşı Dağılımı").FontSize(9).Bold().FontColor("#334155");
                    c.Item().PaddingTop(6).Element(ct => ComposeInsuredAgeDist(ct, _distribution.InsuredAges));
                });
            });
        });
    }

    private void ComposeDistBars(IContainer container, List<DistributionItemDto> items, string barColor)
    {
        if (!items.Any()) return;
        var maxCount = items.Max(x => x.Count);

        container.Table(table =>
        {
            table.ColumnsDefinition(cols =>
            {
                cols.ConstantColumn(70);
                cols.RelativeColumn();
                cols.ConstantColumn(50);
                cols.ConstantColumn(35);
            });

            foreach (var item in items.Take(10))
            {
                var ratio = maxCount > 0 ? (decimal)item.Count / maxCount : 0;
                var barW = (int)(ratio * 100);

                table.Cell().PaddingVertical(2).Text(item.Label).FontSize(7).FontColor("#475569");
                table.Cell().PaddingVertical(2).AlignLeft().Row(r =>
                {
                    r.ConstantItem(barW).Height(10).Background(barColor);
                    r.RelativeItem();
                });
                table.Cell().PaddingVertical(2).AlignRight().Text($"{item.Count:N0}").FontSize(7).FontColor("#1e293b");
                table.Cell().PaddingVertical(2).AlignRight().Text($"%{item.Share:F1}").FontSize(7).FontColor("#64748b");
            }
        });
    }

    private void ComposeStepDist(IContainer container, List<DistributionItemDto> items)
    {
        if (!items.Any()) return;
        var maxCount = items.Max(x => x.Count);

        container.Table(table =>
        {
            table.ColumnsDefinition(cols =>
            {
                cols.ConstantColumn(70);
                cols.RelativeColumn();
                cols.ConstantColumn(50);
                cols.ConstantColumn(35);
            });

            foreach (var item in items.Take(10))
            {
                var ratio = maxCount > 0 ? (decimal)item.Count / maxCount : 0;
                var barW = (int)(ratio * 100);
                var isStep0 = item.Label.Contains("0");
                var barColor = isStep0 ? "#ef4444" : "#10b981";

                if (isStep0)
                    table.Cell().PaddingVertical(2).Text(item.Label).FontSize(7).FontColor("#dc2626").Bold();
                else
                    table.Cell().PaddingVertical(2).Text(item.Label).FontSize(7).FontColor("#475569");
                
                table.Cell().PaddingVertical(2).AlignLeft().Row(r =>
                {
                    r.ConstantItem(barW).Height(10).Background(barColor);
                    r.RelativeItem();
                });
                table.Cell().PaddingVertical(2).AlignRight().Text($"{item.Count:N0}").FontSize(7).FontColor("#1e293b");
                
                if (isStep0)
                    table.Cell().PaddingVertical(2).AlignRight().Text($"%{item.Share:F1}").FontSize(7).FontColor("#dc2626").Bold();
                else
                    table.Cell().PaddingVertical(2).AlignRight().Text($"%{item.Share:F1}").FontSize(7).FontColor("#64748b");
            }
        });
    }

    private void ComposeVehicleAgeDist(IContainer container, List<DistributionItemDto> items)
    {
        if (!items.Any()) return;
        var maxCount = items.Max(x => x.Count);

        container.Table(table =>
        {
            table.ColumnsDefinition(cols =>
            {
                cols.ConstantColumn(50);
                cols.RelativeColumn();
                cols.ConstantColumn(50);
                cols.ConstantColumn(35);
            });

            foreach (var item in items.Take(10))
            {
                var ratio = maxCount > 0 ? (decimal)item.Count / maxCount : 0;
                var barW = (int)(ratio * 100);

                table.Cell().PaddingVertical(2).Text(item.Label).FontSize(7).FontColor("#475569");
                table.Cell().PaddingVertical(2).AlignLeft().Row(r =>
                {
                    r.ConstantItem(barW).Height(10).Background("#10b981");
                    r.RelativeItem();
                });
                table.Cell().PaddingVertical(2).AlignRight().Text($"{item.Count:N0}").FontSize(7).FontColor("#1e293b");
                table.Cell().PaddingVertical(2).AlignRight().Text($"%{item.Share:F1}").FontSize(7).FontColor("#64748b");
            }
        });
    }

    private void ComposeInsuredAgeDist(IContainer container, List<DistributionItemDto> items)
    {
        if (!items.Any()) return;
        var maxCount = items.Max(x => x.Count);

        container.Table(table =>
        {
            table.ColumnsDefinition(cols =>
            {
                cols.ConstantColumn(70);
                cols.RelativeColumn();
                cols.ConstantColumn(50);
                cols.ConstantColumn(35);
            });

            foreach (var item in items.Take(10))
            {
                var ratio = maxCount > 0 ? (decimal)item.Count / maxCount : 0;
                var barW = (int)(ratio * 100);
                var isYoung = item.Label.Contains("18") || item.Label.Contains("25");
                var barColor = isYoung ? "#ef4444" : "#10b981";

                if (isYoung)
                    table.Cell().PaddingVertical(2).Text(item.Label).FontSize(7).FontColor("#dc2626").Bold();
                else
                    table.Cell().PaddingVertical(2).Text(item.Label).FontSize(7).FontColor("#475569");
                
                table.Cell().PaddingVertical(2).AlignLeft().Row(r =>
                {
                    r.ConstantItem(barW).Height(10).Background(barColor);
                    r.RelativeItem();
                });
                table.Cell().PaddingVertical(2).AlignRight().Text($"{item.Count:N0}").FontSize(7).FontColor("#1e293b");
                table.Cell().PaddingVertical(2).AlignRight().Text($"%{item.Share:F1}").FontSize(7).FontColor("#64748b");
            }
        });
    }

    private void ComposeRiskSegments(IContainer container)
    {
        container.Column(col =>
        {
            col.Item().Element(c => SectionLabel(c, "RİSKLİ SEGMENTLER"));

            var segments = _risk.Segments;
            if (!segments.Any()) return;

            var colors = new[] { ("#fef2f2", "#dc2626", "#991b1b"), ("#fff7ed", "#ea580c", "#9a3412"), ("#fefce8", "#ca8a04", "#854d0e"), ("#ecfdf5", "#10b981", "#065f46") };

            col.Item().Row(row =>
            {
                foreach (var (seg, i) in segments.Take(4).Select((s, idx) => (s, idx)))
                {
                    var (bg, accent, text) = colors[i % colors.Length];
                    row.RelativeItem().Background(bg).BorderLeft(3).BorderColor(accent).Padding(10).Column(c =>
                    {
                        c.Item().Text(seg.Name).FontSize(9).Bold().FontColor(text);
                        c.Item().PaddingTop(4).Text(seg.Description).FontSize(8).FontColor(accent);
                        c.Item().PaddingTop(4).Text($"Adet: {seg.PolicyCount:N0}").FontSize(8).Bold().FontColor(text);
                    });
                    if (i < 3) row.ConstantItem(8);
                }
            });
        });
    }

    private void ComposeYoungDriverSection(IContainer container)
    {
        container.Column(col =>
        {
            col.Item().PaddingTop(16).Element(c => SectionLabel(c, "GENÇ SÜRÜCÜ (18-25) × MARKA"));

            var brands = _youngDriver.Brands;
            if (!brands.Any()) return;

            col.Item().Row(row =>
            {
                foreach (var (brand, i) in brands.Take(5).Select((b, idx) => (b, idx)))
                {
                    row.RelativeItem().Background("#fef2f2").Border(0.5f).BorderColor("#fecaca").Padding(10).Column(c =>
                    {
                        c.Item().AlignCenter().Text($"{brand.Count:N0}").FontSize(16).Bold().FontColor("#dc2626");
                        c.Item().AlignCenter().PaddingTop(4).Text(brand.Label).FontSize(8).FontColor("#991b1b");
                    });
                    if (i < 4) row.ConstantItem(8);
                }
            });
        });
    }

    // ===== HELPER METODLAR =====

    private static (string bg, string border, string text) GetLevelColors(string level) => level switch
    {
        "danger" => ("#fef2f2", "#fecaca", "#dc2626"),
        "warning" => ("#fff7ed", "#fed7aa", "#ea580c"),
        "success" => ("#f0fdf4", "#bbf7d0", "#16a34a"),
        _ => ("#f8fafc", "#e2e8f0", "#475569")
    };

    private static (string bg, string border, string iconColor) GetFindingColors(string level) => level switch
    {
        "critical" => ("#fef2f2", "#dc2626", "#dc2626"),
        "high" => ("#fff7ed", "#ea580c", "#ea580c"),
        "medium" => ("#fefce8", "#ca8a04", "#ca8a04"),
        _ => ("#f8fafc", "#64748b", "#64748b")
    };

    private static bool IsPremiumBrand(string brand)
    {
        if (string.IsNullOrEmpty(brand)) return false;
        var upper = brand.ToUpperInvariant();
        return PremiumBrands.Any(p => upper.Contains(p));
    }

    private static string GetHeatmapBg(int value, int min, int max)
    {
        if (value <= 0) return "#ffffff";
        if (max == min) return "#fef9c3";

        var ratio = (double)(value - min) / (max - min);
        if (ratio < 0.33) return "#dcfce7";
        if (ratio < 0.66) return "#fef9c3";
        return "#fee2e2";
    }
}
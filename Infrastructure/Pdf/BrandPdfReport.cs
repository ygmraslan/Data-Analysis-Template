using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using DataAnalysis.Application.Features.Brand.Queries.GetBrandKpi;
using DataAnalysis.Application.Features.Brand.Queries.GetBrandTrend;
using DataAnalysis.Application.Features.Brand.Queries.GetBrandModels;
using DataAnalysis.Application.Features.Brand.Queries.GetBrandHeatmap;

namespace DataAnalysis.Infrastructure.Pdf;

public class BrandPdfReport : BasePdfReport
{
    private readonly BrandKpiResponse _kpi;
    private readonly List<BrandTrendResponse> _trend;
    private readonly List<BrandModelResponse> _models;
    private readonly List<BrandHeatmapResponse> _heatmap;
    private readonly string _brand;

    private static readonly List<string> ChartColors =
    [
        "#10B981", "#3B82F6", "#F59E0B", "#EF4444", "#8B5CF6",
        "#06B6D4", "#F97316", "#EC4899", "#84CC16"
    ];

    public BrandPdfReport(
        BrandKpiResponse kpi,
        List<BrandTrendResponse> trend,
        List<BrandModelResponse> models,
        List<BrandHeatmapResponse> heatmap,
        string productGroup,
        string brand,
        string dateRange,
        string filterSummary = "")
        : base("Marka Analizi Raporu", productGroup, dateRange, filterSummary)
    {
        _kpi     = kpi;
        _trend   = trend;
        _models  = models;
        _heatmap = heatmap;
        _brand   = brand;
    }

    protected override void ComposeContent(IContainer container)
    {
        container.Column(col =>
        {
            col.Spacing(18);
            col.Item().Element(ComposeKpiSection);
            col.Item().Element(ComposeTrendChartSection);
            col.Item().PageBreak();
            col.Item().Element(ComposeTrendTableSection);
            col.Item().PaddingTop(4).Element(ComposeModelsSection);
            col.Item().PageBreak();
            col.Item().Element(ComposeHeatmapSection);
        });
    }

    private void ComposeKpiSection(IContainer container)
    {
        container.Column(col =>
        {
            col.Item().Element(c => SectionLabel(c, "MARKA ÜRETİM KPI"));

            col.Item().Row(row =>
            {
                row.RelativeItem().Element(c => KpiCard(c,
                    "EN YÜKSEK POLİÇE", "#10b981",
                    _kpi.TopBrand, $"{_kpi.TopBrandCount:N0} Poliçe", GetLastWeekRange(),
                    _kpi.TopBrandPrev, $"{_kpi.TopBrandPrevCount:N0} Poliçe", GetPrevWeekRange()));

                row.ConstantItem(10);

                row.RelativeItem().Element(c => KpiCard(c,
                    "EN DÜŞÜK POLİÇE", "#3b82f6",
                    _kpi.BottomBrand, $"{_kpi.BottomBrandCount:N0} Poliçe", GetLastWeekRange(),
                    _kpi.BottomBrandPrev, $"{_kpi.BottomBrandPrevCount:N0} Poliçe", GetPrevWeekRange()));

                row.ConstantItem(10);

                row.RelativeItem().Element(c => ChangeKpiCard(c,
                    "EN YÜKSEK ARTIŞ", "#10b981",
                    _kpi.HasGainer ? _kpi.TopGainerBrand : "—",
                    _kpi.HasGainer ? FmtWoW(_kpi.TopGainerWoW) : null,
                    _kpi.HasGainer ? "#10b981" : null,
                    _kpi.HasGainer ? GetLastWeekRange() : "Bu hafta artış gösteren marka yok",
                    _kpi.PrevTopGainerBrand, FmtWoW(_kpi.PrevTopGainerWoW), GetPrevWeekRange()));

                row.ConstantItem(10);

                row.RelativeItem().Element(c => ChangeKpiCard(c,
                    "EN YÜKSEK AZALIŞ", "#ef4444",
                    _kpi.HasLoser ? _kpi.TopLoserBrand : "—",
                    _kpi.HasLoser ? FmtWoW(_kpi.TopLoserWoW) : null,
                    _kpi.HasLoser ? "#ef4444" : null,
                    _kpi.HasLoser ? GetLastWeekRange() : "Bu hafta tüm markalar artış gösterdi",
                    _kpi.PrevTopLoserBrand, FmtWoW(_kpi.PrevTopLoserWoW), GetPrevWeekRange()));
            });
        });
    }

    private void ComposeTrendChartSection(IContainer container)
    {
        if (!_trend.Any()) return;

        var weeks = _trend.Select(d => d.WeekLabel).ToList();
        var lastWeek = weeks.LastOrDefault() ?? "";

        var svgChart = DrawLineChart(
            new List<string> { _brand },
            weeks,
            (_, w) =>
            {
                var item = _trend.FirstOrDefault(d => d.WeekLabel == w);
                return item?.PolicyCount ?? 0;
            },
            ChartColors);

        container.Column(col =>
        {
            col.Item().Element(c => SectionLabel(c, $"HAFTALIK POLİÇE TRENDİ — {_brand}"));

            col.Item()
                .Border(0.5f).BorderColor("#e2e8f0")
                .Padding(12)
                .Column(inner =>
                {
                    // Özet metrikler
                    var lastData = _trend.LastOrDefault();
                    inner.Item().PaddingBottom(10).Row(row =>
                    {
                        row.RelativeItem().Column(c =>
                        {
                            c.Item().Text("Toplam Poliçe").FontSize(7).FontColor("#64748b");
                            c.Item().Text($"{_trend.Sum(x => x.PolicyCount):N0}").FontSize(12).Bold().FontColor("#0f172a");
                        });
                        row.ConstantItem(1).Background("#e2e8f0");
                        row.RelativeItem().PaddingLeft(12).Column(c =>
                        {
                            c.Item().Text("Net Prim").FontSize(7).FontColor("#64748b");
                            c.Item().Text(FmtPrm(_trend.Sum(x => x.NetPremium))).FontSize(12).Bold().FontColor("#0f172a");
                        });
                        row.ConstantItem(1).Background("#e2e8f0");
                        row.RelativeItem().PaddingLeft(12).Column(c =>
                        {
                            c.Item().Text("Son Hafta WoW").FontSize(7).FontColor("#64748b");
                            var wow = lastData?.WoW ?? 0;
                            c.Item().Text(FmtWoW(wow)).FontSize(12).Bold()
                                .FontColor(wow > 0 ? "#10b981" : wow < 0 ? "#ef4444" : "#94a3b8");
                        });
                    });

                    // Grafik
                    inner.Item().Svg(svgChart);

                });
        });
    }

    private void ComposeTrendTableSection(IContainer container)
    {
        if (!_trend.Any()) return;

        var weeks    = _trend.Select(d => d.WeekLabel).ToList();
        var lastWeek = weeks.LastOrDefault() ?? "";

        container.Column(col =>
        {
            col.Item().Element(c => SectionLabel(c, $"HAFTALIK POLİÇE TRENDİ DETAY — {_brand}"));

            col.Item()
                .Border(0.5f).BorderColor("#e2e8f0")
                .Padding(12)
                .Table(table =>
                    {
                        table.ColumnsDefinition(cols =>
                        {
                            cols.RelativeColumn(2f);
                            cols.RelativeColumn();
                            cols.RelativeColumn();
                            cols.ConstantColumn(44);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Background("#0f3460").PaddingVertical(5).PaddingHorizontal(6)
                                .Text("HAFTA").FontSize(7).Bold().FontColor("#ffffff");
                            header.Cell().Background("#0f3460").PaddingVertical(5).PaddingHorizontal(4).AlignRight()
                                .Text("POLİÇE").FontSize(7).Bold().FontColor("#ffffff");
                            header.Cell().Background("#0f3460").PaddingVertical(5).PaddingHorizontal(4).AlignRight()
                                .Text("NET PRİM").FontSize(7).Bold().FontColor("#ffffff");
                            header.Cell().Background("#0f3460").PaddingVertical(5).PaddingHorizontal(4).AlignCenter()
                                .Text("WoW").FontSize(7).Bold().FontColor("#ffffff");
                        });

                        foreach (var (item, ri) in _trend.Select((x, i) => (x, i)))
                        {
                            var rowBg    = ri % 2 == 0 ? "#ffffff" : "#f8fafc";
                            var isLast   = item.WeekLabel == lastWeek;
                            var wowColor = item.WoW > 0 ? "#10b981" : item.WoW < 0 ? "#ef4444" : "#94a3b8";

                            table.Cell().Background(rowBg).BorderBottom(0.5f).BorderColor("#f1f5f9")
                                .PaddingVertical(5).PaddingHorizontal(6)
                                .Text(item.WeekLabel).FontSize(isLast ? 8 : 7.5f).FontColor("#1e293b");

                            table.Cell().Background(rowBg).BorderBottom(0.5f).BorderColor("#f1f5f9")
                                .PaddingVertical(5).PaddingHorizontal(4).AlignRight()
                                .Text($"{item.PolicyCount:N0}").FontSize(isLast ? 8 : 7.5f)
                                .FontColor(isLast ? "#0f172a" : "#475569");

                            table.Cell().Background(rowBg).BorderBottom(0.5f).BorderColor("#f1f5f9")
                                .PaddingVertical(5).PaddingHorizontal(4).AlignRight()
                                .Text(FmtPrm(item.NetPremium)).FontSize(isLast ? 8 : 7.5f)
                                .FontColor(isLast ? "#0f172a" : "#475569");

                            table.Cell().Background(rowBg).BorderBottom(0.5f).BorderColor("#f1f5f9")
                                .PaddingVertical(5).PaddingHorizontal(4).AlignCenter()
                                .Text(FmtWoW(item.WoW)).FontSize(8).Bold().FontColor(wowColor);
                        }
                    });
        });
    }

    private void ComposeModelsSection(IContainer container)
    {
        if (!_models.Any()) return;

        container.Column(col =>
        {
            col.Item().Element(c => SectionLabel(c, $"MODEL DAĞILIMI — {_brand}"));

            col.Item()
                .Border(0.5f).BorderColor("#e2e8f0")
                .Padding(12)
                .Table(table =>
                {
                    table.ColumnsDefinition(cols =>
                    {
                        cols.RelativeColumn(2.5f);
                        cols.RelativeColumn();
                        cols.RelativeColumn();
                        cols.RelativeColumn();
                        cols.ConstantColumn(44);
                    });

                    table.Header(header =>
                    {
                        header.Cell().Background("#0f3460").PaddingVertical(5).PaddingHorizontal(6)
                            .Text("MODEL").FontSize(7).Bold().FontColor("#ffffff");
                        header.Cell().Background("#0f3460").PaddingVertical(5).PaddingHorizontal(4).AlignRight()
                            .Text("POLİÇE").FontSize(7).Bold().FontColor("#ffffff");
                        header.Cell().Background("#0f3460").PaddingVertical(5).PaddingHorizontal(4).AlignRight()
                            .Text("NET PRİM").FontSize(7).Bold().FontColor("#ffffff");
                        header.Cell().Background("#0f3460").PaddingVertical(5).PaddingHorizontal(4).AlignRight()
                            .Text("ORT. PRİM").FontSize(7).Bold().FontColor("#ffffff");
                        header.Cell().Background("#0f3460").PaddingVertical(5).PaddingHorizontal(4).AlignCenter()
                            .Text("WoW").FontSize(7).Bold().FontColor("#ffffff");
                    });

                    foreach (var (model, ri) in _models.Select((x, i) => (x, i)))
                    {
                        var rowBg    = ri % 2 == 0 ? "#ffffff" : "#f8fafc";
                        var wowColor = model.WoW > 0 ? "#10b981" : model.WoW < 0 ? "#ef4444" : "#94a3b8";

                        table.Cell().Background(rowBg).BorderBottom(0.5f).BorderColor("#f1f5f9")
                            .PaddingVertical(5).PaddingHorizontal(6)
                            .Text(model.Model).FontSize(8).Bold().FontColor("#1e293b");

                        table.Cell().Background(rowBg).BorderBottom(0.5f).BorderColor("#f1f5f9")
                            .PaddingVertical(5).PaddingHorizontal(4).AlignRight()
                            .Text($"{model.PolicyCount:N0}").FontSize(8).FontColor("#475569");

                        table.Cell().Background(rowBg).BorderBottom(0.5f).BorderColor("#f1f5f9")
                            .PaddingVertical(5).PaddingHorizontal(4).AlignRight()
                            .Text(FmtPrm(model.NetPremium)).FontSize(8).FontColor("#475569");

                        table.Cell().Background(rowBg).BorderBottom(0.5f).BorderColor("#f1f5f9")
                            .PaddingVertical(5).PaddingHorizontal(4).AlignRight()
                            .Text(FmtPrm(model.AvgPremium)).FontSize(8).FontColor("#475569");

                        table.Cell().Background(rowBg).BorderBottom(0.5f).BorderColor("#f1f5f9")
                            .PaddingVertical(5).PaddingHorizontal(4).AlignCenter()
                            .Text(FmtWoW(model.WoW)).FontSize(8).Bold().FontColor(wowColor);
                    }
                });
        });
    }

    private void ComposeHeatmapSection(IContainer container)
    {
        if (!_heatmap.Any()) return;

        var brands   = _heatmap.Select(d => d.Brand).Distinct().ToList();
        var weeks    = _heatmap.Select(d => d.Week).Distinct().ToList();
        var mapPrem  = _heatmap.ToDictionary(d => $"{d.Brand}__{d.Week}", d => d.AvgNetPremium);
        var mapRatio = _heatmap.ToDictionary(d => $"{d.Brand}__{d.Week}", d => d.PolicyRatio);

        container.Column(col =>
        {
            // Ort. Yazılan Prim Heatmap
            col.Item().Element(c => SectionLabel(c, "ORT. YAZILAN PRİM HEATMAP"));
            col.Item().Element(c => HeatmapTable(c, brands, weeks,
                (brand, week) => mapPrem.TryGetValue($"{brand}__{week}", out var v) ? v : 0,
                "Marka"));

            // Poliçe Payı Heatmap
            col.Item().PaddingTop(18).Element(c => SectionLabel(c, "POLİÇE PAYI HEATMAP"));
            col.Item().Element(c => HeatmapTableRatio(c, brands, weeks,
                (brand, week) => mapRatio.TryGetValue($"{brand}__{week}", out var v) ? v : 0,
                "Marka"));
        });
    }
}
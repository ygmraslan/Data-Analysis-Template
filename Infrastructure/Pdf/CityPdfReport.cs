using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using DataAnalysis.Application.Features.City.Queries.GetCityKpi;
using DataAnalysis.Application.Features.City.Queries.GetCityList;
using DataAnalysis.Application.Features.City.Queries.GetCityTrend;
using DataAnalysis.Application.Features.City.Queries.GetCityProfile;
using DataAnalysis.Application.Features.City.Queries.GetCityHeatmap;

namespace DataAnalysis.Infrastructure.Pdf;

public class CityPdfReport : BasePdfReport
{
    private readonly CityKpiResponse          _kpi;
    private readonly List<CityListResponse>   _list;
    private readonly List<CityTrendResponse>  _trend;
    private readonly CityProfileResponse      _profile;
    private readonly List<CityHeatmapResponse> _heatmap;

    private static readonly List<string> ChartColors =
    [
        "#10B981", "#3B82F6", "#F59E0B", "#EF4444", "#8B5CF6",
        "#06B6D4", "#F97316", "#EC4899", "#84CC16"
    ];

    public CityPdfReport(
        CityKpiResponse kpi,
        List<CityListResponse> list,
        List<CityTrendResponse> trend,
        CityProfileResponse profile,
        List<CityHeatmapResponse> heatmap,
        string productGroup,
        string dateRange,
        string filterSummary = "")
        : base("Coğrafi Analiz Raporu", productGroup, dateRange, filterSummary)
    {
        _kpi     = kpi;
        _list    = list;
        _trend   = trend;
        _profile = profile;
        _heatmap = heatmap;
    }

    protected override void ComposeContent(IContainer container)
    {
        container.Column(col =>
        {
            col.Spacing(18);
            col.Item().Element(ComposeKpiSection);
            col.Item().Element(ComposeListSection);
            col.Item().PageBreak();
            col.Item().Element(ComposeTrendChartSection);
            col.Item().Element(ComposeTrendTableSection);
            col.Item().Element(ComposeProfileSection);
            col.Item().PageBreak();
            col.Item().Element(ComposeHeatmapSection);
        });
    }

    private void ComposeKpiSection(IContainer container)
    {
        container.Column(col =>
        {
            col.Item().Element(c => SectionLabel(c, "İL ÜRETİM KPI"));

            col.Item().Row(row =>
            {
                row.RelativeItem().Element(c => KpiCard(c,
                    "EN YÜKSEK ÜRETİM", "#10b981",
                    _kpi.TopCity, FmtPrm(_kpi.TopCityPremium), GetLastWeekRange(),
                    _kpi.TopCityPrev, FmtPrm(_kpi.TopCityPrevPremium), GetPrevWeekRange()));

                row.ConstantItem(10);

                row.RelativeItem().Element(c => KpiCard(c,
                    "EN DÜŞÜK ÜRETİM", "#3b82f6",
                    _kpi.BottomCity, FmtPrm(_kpi.BottomCityPremium), GetLastWeekRange(),
                    _kpi.BottomCityPrev, FmtPrm(_kpi.BottomCityPrevPremium), GetPrevWeekRange()));

                row.ConstantItem(10);

                row.RelativeItem().Element(c => ChangeKpiCard(c,
                    "EN YÜKSEK ARTIŞ", "#10b981",
                    _kpi.HasGainer ? _kpi.TopGainerCity : "—",
                    _kpi.HasGainer ? FmtWoW(_kpi.TopGainerWoW) : null,
                    _kpi.HasGainer ? "#10b981" : null,
                    _kpi.HasGainer ? GetLastWeekRange() : "Bu hafta artış gösteren il yok",
                    _kpi.PrevTopGainerCity, FmtWoW(_kpi.PrevTopGainerWoW), GetPrevWeekRange()));

                row.ConstantItem(10);

                row.RelativeItem().Element(c => ChangeKpiCard(c,
                    "EN YÜKSEK AZALIŞ", "#ef4444",
                    _kpi.HasLoser ? _kpi.TopLoserCity : "—",
                    _kpi.HasLoser ? FmtWoW(_kpi.TopLoserWoW) : null,
                    _kpi.HasLoser ? "#ef4444" : null,
                    _kpi.HasLoser ? GetLastWeekRange() : "Bu hafta tüm iller artış gösterdi",
                    _kpi.PrevTopLoserCity, FmtWoW(_kpi.PrevTopLoserWoW), GetPrevWeekRange()));
            });
        });
    }

    private void ComposeListSection(IContainer container)
    {
        if (!_list.Any()) return;

        container.Column(col =>
        {
            col.Item().Element(c => SectionLabel(c, "İL BAZLI ÜRETİM SIRALAMASI"));

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
                            .Text("İL").FontSize(7).Bold().FontColor("#ffffff");
                        header.Cell().Background("#0f3460").PaddingVertical(5).PaddingHorizontal(4).AlignRight()
                            .Text("POLİÇE").FontSize(7).Bold().FontColor("#ffffff");
                        header.Cell().Background("#0f3460").PaddingVertical(5).PaddingHorizontal(4).AlignRight()
                            .Text("NET PRİM").FontSize(7).Bold().FontColor("#ffffff");
                        header.Cell().Background("#0f3460").PaddingVertical(5).PaddingHorizontal(4).AlignRight()
                            .Text("ORT. PRİM").FontSize(7).Bold().FontColor("#ffffff");
                        header.Cell().Background("#0f3460").PaddingVertical(5).PaddingHorizontal(4).AlignCenter()
                            .Text("WoW").FontSize(7).Bold().FontColor("#ffffff");
                    });

                    foreach (var (item, ri) in _list.Select((x, i) => (x, i)))
                    {
                        var rowBg    = ri % 2 == 0 ? "#ffffff" : "#f8fafc";
                        var wowColor = item.WoW > 0 ? "#10b981" : item.WoW < 0 ? "#ef4444" : "#94a3b8";

                        table.Cell().Background(rowBg).BorderBottom(0.5f).BorderColor("#f1f5f9")
                            .PaddingVertical(5).PaddingHorizontal(6)
                            .Text(item.City).FontSize(8).Bold().FontColor("#1e293b");

                        table.Cell().Background(rowBg).BorderBottom(0.5f).BorderColor("#f1f5f9")
                            .PaddingVertical(5).PaddingHorizontal(4).AlignRight()
                            .Text($"{item.PolicyCount:N0}").FontSize(8).FontColor("#475569");

                        table.Cell().Background(rowBg).BorderBottom(0.5f).BorderColor("#f1f5f9")
                            .PaddingVertical(5).PaddingHorizontal(4).AlignRight()
                            .Text(FmtPrm(item.NetPremium)).FontSize(8).FontColor("#475569");

                        table.Cell().Background(rowBg).BorderBottom(0.5f).BorderColor("#f1f5f9")
                            .PaddingVertical(5).PaddingHorizontal(4).AlignRight()
                            .Text(FmtPrm(item.AvgPremium)).FontSize(8).FontColor("#475569");

                        table.Cell().Background(rowBg).BorderBottom(0.5f).BorderColor("#f1f5f9")
                            .PaddingVertical(5).PaddingHorizontal(4).AlignCenter()
                            .Text(FmtWoW(item.WoW)).FontSize(8).Bold().FontColor(wowColor);
                    }
                });
        });
    }

    private void ComposeTrendChartSection(IContainer container)
    {
        if (!_trend.Any()) return;

        var city  = _kpi.DefaultCity;
        var weeks = _trend.Select(d => d.WeekLabel).ToList();

        var svgChart = DrawLineChart(
            new List<string> { city },
            weeks,
            (_, w) =>
            {
                var item = _trend.FirstOrDefault(d => d.WeekLabel == w);
                return item?.NetPremium ?? 0;
            },
            ChartColors);

        container.Column(col =>
        {
            col.Item().Element(c => SectionLabel(c, $"HAFTALIK NET PRİM TRENDİ — {city}"));

            col.Item()
                .Border(0.5f).BorderColor("#e2e8f0")
                .Padding(12)
                .Column(inner =>
                {
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

                    inner.Item().Svg(svgChart);
                });
        });
    }

    private void ComposeTrendTableSection(IContainer container)
    {
        if (!_trend.Any()) return;

        var city     = _kpi.HasGainer ? _kpi.TopGainerCity : _kpi.DefaultCity;
        var weeks    = _trend.Select(d => d.WeekLabel).ToList();
        var lastWeek = weeks.LastOrDefault() ?? "";

        container.Column(col =>
        {
            col.Item().Element(c => SectionLabel(c, $"HAFTALIK TREND DETAY — {city}"));

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

    private void ComposeProfileSection(IContainer container)
    {
        if (_profile == null) return;

        var city = _kpi.DefaultCity;

        var topBrands   = _profile.TopBrands;
        var basamak     = _profile.Profile.Where(x => x.Category == "Basamak").OrderBy(x => x.Type).ToList();
        var aracYasi    = _profile.Profile.Where(x => x.Category == "AracYasi").OrderBy(x => x.Type).ToList();
        var sigortaliTuru = _profile.Profile.Where(x => x.Category == "SigortaliTuru").ToList();

        container.Column(col =>
        {
            col.Item().Element(c => SectionLabel(c, $"İL PROFİLİ — {city}"));

            col.Item().Row(row =>
            {
                // Top 3 Marka
                row.RelativeItem().Border(0.5f).BorderColor("#e2e8f0").Padding(10).Column(inner =>
                {
                    inner.Item().PaddingBottom(8).Text("TOP 3 MARKA")
                        .FontSize(7).Bold().FontColor("#64748b").LetterSpacing(0.05f);

                    foreach (var (brand, ri) in topBrands.Select((b, i) => (b, i)))
                    {
                        var rowBg = ri % 2 == 0 ? "#ffffff" : "#f8fafc";
                        inner.Item().Background(rowBg).BorderBottom(0.5f).BorderColor("#f1f5f9")
                            .PaddingVertical(5).Row(r =>
                            {
                                r.RelativeItem().Text(brand.Brand).FontSize(8).Bold().FontColor("#1e293b");
                                r.AutoItem().Text($"{brand.PolicyCount:N0} Poliçe").FontSize(8).FontColor("#475569");
                            });
                    }
                });

                row.ConstantItem(10);

                // Sigortalı Türü
                row.RelativeItem().Border(0.5f).BorderColor("#e2e8f0").Padding(10).Column(inner =>
                {
                    inner.Item().PaddingBottom(8).Text("SİGORTALI TÜRÜ")
                        .FontSize(7).Bold().FontColor("#64748b").LetterSpacing(0.05f);

                    var total = sigortaliTuru.Sum(x => x.PolicyCount);
                    foreach (var (item, ri) in sigortaliTuru.Select((x, i) => (x, i)))
                    {
                        var ratio  = total > 0 ? (float)item.PolicyCount / total * 100 : 0;
                        var rowBg  = ri % 2 == 0 ? "#ffffff" : "#f8fafc";
                        inner.Item().Background(rowBg).BorderBottom(0.5f).BorderColor("#f1f5f9")
                            .PaddingVertical(5).Row(r =>
                            {
                                r.RelativeItem().Text(item.Type).FontSize(8).Bold().FontColor("#1e293b");
                                r.AutoItem().Text($"%{ratio:F1}").FontSize(8).FontColor("#475569");
                            });
                    }
                });

                row.ConstantItem(10);

                // Araç Yaşı
                row.RelativeItem().Border(0.5f).BorderColor("#e2e8f0").Padding(10).Column(inner =>
                {
                    inner.Item().PaddingBottom(8).Text("ARAÇ YAŞI DAĞILIMI")
                        .FontSize(7).Bold().FontColor("#64748b").LetterSpacing(0.05f);

                    foreach (var (item, ri) in aracYasi.Select((x, i) => (x, i)))
                    {
                        var rowBg = ri % 2 == 0 ? "#ffffff" : "#f8fafc";
                        inner.Item().Background(rowBg).BorderBottom(0.5f).BorderColor("#f1f5f9")
                            .PaddingVertical(5).Row(r =>
                            {
                                r.RelativeItem().Text(item.Type).FontSize(8).Bold().FontColor("#1e293b");
                                r.AutoItem().Text($"{item.PolicyCount:N0}").FontSize(8).FontColor("#475569");
                            });
                    }
                });

                row.ConstantItem(10);

                // Basamak
                row.RelativeItem().Border(0.5f).BorderColor("#e2e8f0").Padding(10).Column(inner =>
                {
                    inner.Item().PaddingBottom(8).Text("BASAMAK DAĞILIMI")
                        .FontSize(7).Bold().FontColor("#64748b").LetterSpacing(0.05f);

                    foreach (var (item, ri) in basamak.Select((x, i) => (x, i)))
                    {
                        var rowBg = ri % 2 == 0 ? "#ffffff" : "#f8fafc";
                        inner.Item().Background(rowBg).BorderBottom(0.5f).BorderColor("#f1f5f9")
                            .PaddingVertical(5).Row(r =>
                            {
                                r.RelativeItem().Text($"Basamak {item.Type}").FontSize(8).Bold().FontColor("#1e293b");
                                r.AutoItem().Text($"{item.PolicyCount:N0}").FontSize(8).FontColor("#475569");
                            });
                    }
                });
            });
        });
    }

    private void ComposeHeatmapSection(IContainer container)
    {
        if (!_heatmap.Any()) return;

        var cities   = _heatmap.Select(d => d.City).Distinct().ToList();
        var weeks    = _heatmap.Select(d => d.Week).Distinct().ToList();
        var mapPrem  = _heatmap.ToDictionary(d => $"{d.City}__{d.Week}", d => d.AvgNetPremium);
        var mapRatio = _heatmap.ToDictionary(d => $"{d.City}__{d.Week}", d => d.PolicyRatio);

        container.Column(col =>
        {
            // Ort. Yazılan Prim Heatmap
            col.Item().Element(c => SectionLabel(c, "ORT. YAZILAN PRİM HEATMAP"));
            col.Item().Element(c => HeatmapTable(c, cities, weeks,
                (city, week) => mapPrem.TryGetValue($"{city}__{week}", out var v) ? v : 0,
                "İl"));

            // Poliçe Payı Heatmap
            col.Item().PaddingTop(18).Element(c => SectionLabel(c, "POLİÇE PAYI HEATMAP"));
            col.Item().Element(c => HeatmapTableRatio(c, cities, weeks,
                (city, week) => mapRatio.TryGetValue($"{city}__{week}", out var v) ? v : 0,
                "İl"));
        });
    }
}
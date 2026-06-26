using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using DataAnalysis.Application.Features.Region.Queries.GetRegionKpi;
using DataAnalysis.Application.Features.Region.Queries.GetRegionTrend;
using DataAnalysis.Application.Features.Region.Queries.GetRegionHeatmap;

namespace DataAnalysis.Infrastructure.Pdf;

public class RegionPdfReport : BasePdfReport
{
    private readonly GetRegionKpiQueryResponse _kpi;
    private readonly List<GetRegionTrendQueryResponse> _trend;
    private readonly List<GetRegionHeatmapQueryResponse> _heatmap;

    private static readonly List<string> ChartColors =
    [
        "#3B82F6", "#10B981", "#F59E0B", "#EF4444", "#8B5CF6",
        "#06B6D4", "#F97316", "#EC4899", "#84CC16"
    ];

    public RegionPdfReport(
        GetRegionKpiQueryResponse kpi,
        List<GetRegionTrendQueryResponse> trend,
        List<GetRegionHeatmapQueryResponse> heatmap,
       string productGroup,
        string dateRange,
        string filterSummary = "")
        : base("Bölge Analizi Raporu", productGroup, dateRange, filterSummary)
    {
        _kpi     = kpi;
        _trend   = trend;
        _heatmap = heatmap;
    }

    protected override void ComposeContent(IContainer container)
    {
        var regions  = _trend.Select(d => d.Region).Distinct().OrderBy(r => r).ToList();
        var weeks    = _trend.Select(d => d.WeekLabel).Distinct().ToList();
        var lastWeek = weeks.LastOrDefault() ?? "";
        var map      = _trend.ToDictionary(d => $"{d.Region}__{d.WeekLabel}", d => d);

        container.Column(col =>
        {
            col.Spacing(18);

            // ── Sayfa 1: KPI + Grafik ──────────────────────────────────────
            col.Item().Element(c => ComposeKpiSection(c));
            col.Item().Element(c => ComposeTrendChartSection(c, regions, weeks, map));

            // ── Sayfa 2: Trend Tablosu + Heatmap ──────────────────────────
            col.Item().PageBreak();
            col.Item().Element(c => ComposeTrendTableSection(c, regions, weeks, lastWeek, map));
            col.Item().PaddingTop(18).Element(c => ComposeHeatmapSection(c));
        });
    }

    private void ComposeKpiSection(IContainer container)
    {
        container.Column(col =>
        {
            col.Item().Element(c => SectionLabel(c, "BÖLGE ÜRETİMİ"));

            col.Item().Row(row =>
            {
                row.RelativeItem().Element(c => KpiCard(c,
                    "EN YÜKSEK PRİM", "#10b981",
                    _kpi.TopRegion, FmtPrm(_kpi.TopRegionPremium), GetLastWeekRange(),
                    _kpi.TopRegionPrev, FmtPrm(_kpi.TopRegionPrevPremium), GetPrevWeekRange()));

                row.ConstantItem(10);

                row.RelativeItem().Element(c => KpiCard(c,
                    "EN DÜŞÜK PRİM", "#3b82f6",
                    _kpi.BottomRegion, FmtPrm(_kpi.BottomRegionPremium), GetLastWeekRange(),
                    _kpi.BottomRegionPrev, FmtPrm(_kpi.BottomRegionPrevPremium), GetPrevWeekRange()));

                row.ConstantItem(10);

                row.RelativeItem().Element(c => ChangeKpiCard(c,
                    "EN YÜKSEK ARTIŞ", "#10b981",
                    _kpi.HasGainer ? _kpi.TopGainerRegion : "—",
                    _kpi.HasGainer ? FmtWoW(_kpi.TopGainerWoW) : null,
                    _kpi.HasGainer ? "#10b981" : null,
                    _kpi.HasGainer ? GetLastWeekRange() : "Bu hafta artış gösteren bölge yok",
                    _kpi.PrevTopGainerRegion, FmtWoW(_kpi.PrevTopGainerWoW), GetPrevWeekRange()));

                row.ConstantItem(10);

                row.RelativeItem().Element(c => ChangeKpiCard(c,
                    "EN YÜKSEK AZALIŞ", "#ef4444",
                    _kpi.HasLoser ? _kpi.TopLoserRegion : "—",
                    _kpi.HasLoser ? FmtWoW(_kpi.TopLoserWoW) : null,
                    _kpi.HasLoser ? "#ef4444" : null,
                    _kpi.HasLoser ? GetLastWeekRange() : "Bu hafta tüm bölgeler artış gösterdi",
                    _kpi.PrevTopLoserRegion, FmtWoW(_kpi.PrevTopLoserWoW), GetPrevWeekRange()));
            });
        });
    }

    private void ComposeTrendChartSection(IContainer container,
        List<string> regions, List<string> weeks,
        Dictionary<string, GetRegionTrendQueryResponse> map)
    {
        var svg = DrawLineChart(
            regions, weeks,
            (r, w) => map.TryGetValue($"{r}__{w}", out var d) ? d.TotalPremium : 0,
            ChartColors);

        container.Column(col =>
        {
            col.Item().Element(c => SectionLabel(c, "HAFTALIK NET PRİM TRENDİ"));

            // Legend
            col.Item().PaddingBottom(8).Row(legendRow =>
            {
                foreach (var (region, ri) in regions.Select((r, i) => (r, i)))
                {
                    legendRow.AutoItem().PaddingRight(10).Row(item =>
                    {
                        item.ConstantItem(16).AlignMiddle().Height(2)
                            .Background(ChartColors[ri % ChartColors.Count]);
                        item.AutoItem().PaddingLeft(4)
                            .Text(region).FontSize(7).FontColor("#475569");
                    });
                }
            });

            // SVG Grafik
            col.Item().Svg(svg);
        });
    }

    private static void ComposeTrendTableSection(IContainer container,
        List<string> regions, List<string> weeks, string lastWeek,
        Dictionary<string, GetRegionTrendQueryResponse> map)
    {
        container.Column(col =>
        {
            col.Item().Element(c => SectionLabel(c, "HAFTALIK NET PRİM TRENDİ — DETAY"));

            col.Item().Table(table =>
            {
                table.ColumnsDefinition(cols =>
                {
                    cols.RelativeColumn(2f);
                    foreach (var _ in weeks) cols.RelativeColumn();
                    cols.ConstantColumn(44);
                });

                table.Header(header =>
                {
                    header.Cell().Background("#0f3460").PaddingVertical(5).PaddingHorizontal(6)
                        .Text("BÖLGE").FontSize(7).Bold().FontColor("#ffffff");

                    foreach (var week in weeks)
                        header.Cell().Background("#0f3460").PaddingVertical(5).PaddingHorizontal(4).AlignRight()
                            .Text(week).FontSize(6.5f).Bold()
                            .FontColor(week == lastWeek ? "#fcd34d" : "#ffffff");

                    header.Cell().Background("#0f3460").PaddingVertical(5).PaddingHorizontal(4).AlignCenter()
                        .Text("WoW").FontSize(7).Bold().FontColor("#ffffff");
                });

                foreach (var (region, ri) in regions.Select((r, i) => (r, i)))
                {
                    var rowBg = ri % 2 == 0 ? "#ffffff" : "#f8fafc";

                    table.Cell().Background(rowBg).BorderBottom(0.5f).BorderColor("#f1f5f9")
                        .PaddingVertical(5).PaddingHorizontal(6)
                        .Text(region).FontSize(8).Bold().FontColor("#1e293b");

                    foreach (var week in weeks)
                    {
                        var key    = $"{region}__{week}";
                        var val    = map.TryGetValue(key, out var d) ? d.TotalPremium : 0;
                        var isLast = week == lastWeek;

                        table.Cell().Background(rowBg).BorderBottom(0.5f).BorderColor("#f1f5f9")
                            .PaddingVertical(5).PaddingHorizontal(4).AlignRight()
                            .Text(val > 0 ? $"₺{(val / 1_000_000m):F2}M" : "—")
                            .FontSize(isLast ? 8 : 7.5f)
                            .FontColor(isLast ? "#0f172a" : "#475569");
                    }

                    var lastData = map.TryGetValue($"{region}__{lastWeek}", out var ld) ? ld : null;
                    var wow      = lastData?.WoW ?? 0;
                    var wowColor = wow > 0 ? "#10b981" : wow < 0 ? "#ef4444" : "#94a3b8";

                    table.Cell().Background(rowBg).BorderBottom(0.5f).BorderColor("#f1f5f9")
                        .PaddingVertical(5).PaddingHorizontal(4).AlignCenter()
                        .Text(FmtWoW(wow)).FontSize(8).Bold().FontColor(wowColor);
                }
            });
        });
    }

    private void ComposeHeatmapSection(IContainer container)
    {
        var regions   = _heatmap.Select(d => d.Region).Distinct().ToList();
        var weeks     = _heatmap.Select(d => d.Week).Distinct().ToList();
        var mapPrem   = _heatmap.ToDictionary(d => $"{d.Region}__{d.Week}", d => d.AvgNetPremium);
        var mapRatio  = _heatmap.ToDictionary(d => $"{d.Region}__{d.Week}", d => d.PolicyRatio);

        container.Column(col =>
        {
            col.Item().Element(c => SectionLabel(c, "ORT. YAZILAN PRİM HEATMAP"));
            col.Item().Element(c => HeatmapTable(c, regions, weeks,
                (region, week) => mapPrem.TryGetValue($"{region}__{week}", out var v) ? v : 0,
                "Bölge"));

            col.Item().PaddingTop(18).Element(c => SectionLabel(c, "POLİÇE PAYI HEATMAP"));
            col.Item().Element(c => HeatmapTableRatio(c, regions, weeks,
                (region, week) => mapRatio.TryGetValue($"{region}__{week}", out var v) ? v : 0,
                "Bölge"));
        });
    }
}
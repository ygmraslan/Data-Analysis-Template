using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using DataAnalysis.Application.Features.Vehicle.Queries.GetVehicleKpi;
using DataAnalysis.Application.Features.Vehicle.Queries.GetVehicleAge;
using DataAnalysis.Application.Features.Vehicle.Queries.GetVehiclePrice;
using DataAnalysis.Application.Features.Vehicle.Queries.GetVehicleBody;
using DataAnalysis.Application.Features.Vehicle.Queries.GetVehicleSegment;
using DataAnalysis.Application.Features.Vehicle.Queries.GetVehicleHeatmap;
using DataAnalysis.Application.Features.Vehicle.Queries.GetVehicleTrend;

namespace DataAnalysis.Infrastructure.Pdf;

public class VehiclePdfReport : BasePdfReport
{
    private readonly VehicleKpiResponse           _kpi;
    private readonly List<VehicleAgeResponse>     _age;
    private readonly List<VehiclePriceResponse>   _price;
    private readonly List<VehicleBodyResponse>    _body;
    private readonly List<VehicleSegmentResponse> _segment;
    private readonly List<VehicleHeatmapResponse> _ageHeatmap;
    private readonly List<VehicleHeatmapResponse> _priceHeatmap;
    private readonly List<VehicleTrendResponse>   _ageTrend;
    private readonly List<VehicleTrendResponse>   _priceTrend;

    private static readonly List<string> ChartColors = ["#10b981", "#3b82f6", "#f59e0b", "#8b5cf6", "#ef4444", "#06b6d4"];

    public VehiclePdfReport(
        VehicleKpiResponse kpi,
        List<VehicleAgeResponse> age,
        List<VehiclePriceResponse> price,
        List<VehicleBodyResponse> body,
        List<VehicleSegmentResponse> segment,
        List<VehicleHeatmapResponse> ageHeatmap,
        List<VehicleHeatmapResponse> priceHeatmap,
        List<VehicleTrendResponse> ageTrend,
        List<VehicleTrendResponse> priceTrend,
        string productGroup,
        string dateRange,
        string filterSummary = "")
        : base("Araç Analizi Raporu", productGroup, dateRange, filterSummary)
    {
        _kpi          = kpi;
        _age          = age;
        _price        = price;
        _body         = body;
        _segment      = segment;
        _ageHeatmap   = ageHeatmap;
        _priceHeatmap = priceHeatmap;
        _ageTrend     = ageTrend;
        _priceTrend   = priceTrend;
    }

    protected override void ComposeContent(IContainer container)
    {
        container.Column(col =>
        {
            col.Spacing(18);
            col.Item().Element(ComposeKpiSection);
            col.Item().Element(ComposeAgeAndPriceSection);
            col.Item().PageBreak();
            col.Item().Element(ComposeBodyAndSegmentSection);
            col.Item().PageBreak();
            col.Item().Element(ComposeAgeHeatmapSection);
            col.Item().Element(ComposePriceHeatmapSection);
        });
    }

    private void ComposeKpiSection(IContainer container)
    {
        container.Column(col =>
        {
            col.Item().Element(c => SectionLabel(c, "ARAÇ ÜRETİM KPI"));
            col.Item().Row(row =>
            {
                row.RelativeItem().Element(c => KpiCard(c,
                    "EN YÜKSEK ÜRETİM — YAŞ", "#10b981",
                    _kpi.TopGainerAge, FmtPrm(_kpi.TopGainerAgeWoW), GetLastWeekRange(),
                    _kpi.PrevTopGainerAge, FmtWoW(_kpi.PrevTopGainerAgeWoW), GetPrevWeekRange()));
                row.ConstantItem(10);
                row.RelativeItem().Element(c => KpiCard(c,
                    "EN DÜŞÜK ÜRETİM — YAŞ", "#3b82f6",
                    _kpi.TopLoserAge, FmtWoW(_kpi.TopLoserAgeWoW), GetLastWeekRange(),
                    _kpi.PrevTopLoserAge, FmtWoW(_kpi.PrevTopLoserAgeWoW), GetPrevWeekRange()));
                row.ConstantItem(10);
                row.RelativeItem().Element(c => KpiCard(c,
                    "EN YÜKSEK ÜRETİM — BEDEL", "#10b981",
                    _kpi.TopGainerPrice, FmtWoW(_kpi.TopGainerPriceWoW), GetLastWeekRange(),
                    _kpi.PrevTopGainerPrice, FmtWoW(_kpi.PrevTopGainerPriceWoW), GetPrevWeekRange()));
                row.ConstantItem(10);
                row.RelativeItem().Element(c => KpiCard(c,
                    "EN DÜŞÜK ÜRETİM — BEDEL", "#f59e0b",
                    _kpi.TopLoserPrice, FmtWoW(_kpi.TopLoserPriceWoW), GetLastWeekRange(),
                    _kpi.PrevTopLoserPrice, FmtWoW(_kpi.PrevTopLoserPriceWoW), GetPrevWeekRange()));
            });
        });
    }

    private void ComposeAgeAndPriceSection(IContainer container)
    {
        var ageGroup   = _kpi.TopGainerAge   ?? string.Empty;
        var priceGroup = _kpi.TopGainerPrice ?? string.Empty;

        var ageWeeks   = _ageTrend.Select(d => d.WeekLabel).ToList();
        var priceWeeks = _priceTrend.Select(d => d.WeekLabel).ToList();

        var ageSvg   = DrawLineChart(new List<string> { ageGroup },   ageWeeks,   (_, w) => _ageTrend.FirstOrDefault(d => d.WeekLabel == w)?.NetPremium   ?? 0, ChartColors);
        var priceSvg = DrawLineChart(new List<string> { priceGroup }, priceWeeks, (_, w) => _priceTrend.FirstOrDefault(d => d.WeekLabel == w)?.NetPremium ?? 0, ChartColors);

        container.Column(col =>
        {
            col.Item().Element(c => SectionLabel(c, "ARAÇ YAŞ & BEDEL TREND"));
            col.Item().Row(row =>
            {
                // Yaş Trend
                row.RelativeItem().Border(0.5f).BorderColor("#e2e8f0").Padding(10).Column(inner =>
                {
                    inner.Item().PaddingBottom(6).Text($"Yaş Grubu Trendi — {ageGroup}").FontSize(9).Bold().FontColor("#0f172a");
                    inner.Item().Row(r =>
                    {
                        r.RelativeItem().Column(c =>
                        {
                            c.Item().Text("Toplam Poliçe").FontSize(7).FontColor("#64748b");
                            c.Item().Text($"{_ageTrend.Sum(x => x.PolicyCount):N0}").FontSize(11).Bold().FontColor("#0f172a");
                        });
                        r.ConstantItem(1).Background("#e2e8f0");
                        r.RelativeItem().PaddingLeft(8).Column(c =>
                        {
                            c.Item().Text("Net Prim").FontSize(7).FontColor("#64748b");
                            c.Item().Text(FmtPrm(_ageTrend.Sum(x => x.NetPremium))).FontSize(11).Bold().FontColor("#0f172a");
                        });
                        r.ConstantItem(1).Background("#e2e8f0");
                        r.RelativeItem().PaddingLeft(8).Column(c =>
                        {
                            c.Item().Text("Son Hafta WoW").FontSize(7).FontColor("#64748b");
                            var wow = _ageTrend.LastOrDefault()?.WoW ?? 0;
                            c.Item().Text(FmtWoW(wow)).FontSize(11).Bold().FontColor(wow > 0 ? "#10b981" : wow < 0 ? "#ef4444" : "#94a3b8");
                        });
                    });
                    inner.Item().PaddingTop(8).Svg(ageSvg);
                });

                row.ConstantItem(12);

                // Bedel Trend
                row.RelativeItem().Border(0.5f).BorderColor("#e2e8f0").Padding(10).Column(inner =>
                {
                    inner.Item().PaddingBottom(6).Text($"Bedel Aralığı Trendi — {priceGroup}").FontSize(9).Bold().FontColor("#0f172a");
                    inner.Item().Row(r =>
                    {
                        r.RelativeItem().Column(c =>
                        {
                            c.Item().Text("Toplam Poliçe").FontSize(7).FontColor("#64748b");
                            c.Item().Text($"{_priceTrend.Sum(x => x.PolicyCount):N0}").FontSize(11).Bold().FontColor("#0f172a");
                        });
                        r.ConstantItem(1).Background("#e2e8f0");
                        r.RelativeItem().PaddingLeft(8).Column(c =>
                        {
                            c.Item().Text("Net Prim").FontSize(7).FontColor("#64748b");
                            c.Item().Text(FmtPrm(_priceTrend.Sum(x => x.NetPremium))).FontSize(11).Bold().FontColor("#0f172a");
                        });
                        r.ConstantItem(1).Background("#e2e8f0");
                        r.RelativeItem().PaddingLeft(8).Column(c =>
                        {
                            c.Item().Text("Son Hafta WoW").FontSize(7).FontColor("#64748b");
                            var wow = _priceTrend.LastOrDefault()?.WoW ?? 0;
                            c.Item().Text(FmtWoW(wow)).FontSize(11).Bold().FontColor(wow > 0 ? "#10b981" : wow < 0 ? "#ef4444" : "#94a3b8");
                        });
                    });
                    inner.Item().PaddingTop(8).Svg(priceSvg);
                });
            });
        });
    }

    private void ComposeBodyAndSegmentSection(IContainer container)
    {
        container.Column(col =>
        {
            col.Item().Element(c => SectionLabel(c, "GÖVDE TİPİ & SEGMENT DAĞILIMI"));
            col.Item().Row(row =>
            {
                row.RelativeItem().Element(ComposeBodyTable);
                row.ConstantItem(12);
                row.RelativeItem().Element(ComposeSegmentTable);
            });
        });
    }

    private void ComposeBodyTable(IContainer container)
    {
        container.Column(col =>
        {
            col.Item().PaddingBottom(6).Text("Gövde Tipi Dağılımı").FontSize(9).Bold().FontColor("#0f172a");
            col.Item().Table(table =>
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
                    header.Cell().Background("#0f3460").PaddingVertical(5).PaddingHorizontal(6).Text("GÖVDE TİPİ").FontSize(7).Bold().FontColor("#ffffff");
                    header.Cell().Background("#0f3460").PaddingVertical(5).PaddingHorizontal(4).AlignRight().Text("POLİÇE").FontSize(7).Bold().FontColor("#ffffff");
                    header.Cell().Background("#0f3460").PaddingVertical(5).PaddingHorizontal(4).AlignRight().Text("NET PRİM").FontSize(7).Bold().FontColor("#ffffff");
                    header.Cell().Background("#0f3460").PaddingVertical(5).PaddingHorizontal(4).AlignCenter().Text("WoW").FontSize(7).Bold().FontColor("#ffffff");
                });
                foreach (var (item, ri) in _body.Select((x, i) => (x, i)))
                {
                    var rowBg    = ri % 2 == 0 ? "#ffffff" : "#f8fafc";
                    var wowColor = item.WoW > 0 ? "#10b981" : item.WoW < 0 ? "#ef4444" : "#94a3b8";
                    table.Cell().Background(rowBg).BorderBottom(0.5f).BorderColor("#f1f5f9").PaddingVertical(5).PaddingHorizontal(6).Text(item.BodyType).FontSize(8).Bold().FontColor("#1e293b");
                    table.Cell().Background(rowBg).BorderBottom(0.5f).BorderColor("#f1f5f9").PaddingVertical(5).PaddingHorizontal(4).AlignRight().Text($"{item.PolicyCount:N0}").FontSize(8).FontColor("#475569");
                    table.Cell().Background(rowBg).BorderBottom(0.5f).BorderColor("#f1f5f9").PaddingVertical(5).PaddingHorizontal(4).AlignRight().Text(FmtPrm(item.NetPremium)).FontSize(8).FontColor("#475569");
                    table.Cell().Background(rowBg).BorderBottom(0.5f).BorderColor("#f1f5f9").PaddingVertical(5).PaddingHorizontal(4).AlignCenter().Text(FmtWoW(item.WoW)).FontSize(8).Bold().FontColor(wowColor);
                }
            });
        });
    }

    private void ComposeSegmentTable(IContainer container)
    {
        container.Column(col =>
        {
            col.Item().PaddingBottom(6).Text("Segment Dağılımı").FontSize(9).Bold().FontColor("#0f172a");
            col.Item().Table(table =>
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
                    header.Cell().Background("#0f3460").PaddingVertical(5).PaddingHorizontal(6).Text("SEGMENT").FontSize(7).Bold().FontColor("#ffffff");
                    header.Cell().Background("#0f3460").PaddingVertical(5).PaddingHorizontal(4).AlignRight().Text("POLİÇE").FontSize(7).Bold().FontColor("#ffffff");
                    header.Cell().Background("#0f3460").PaddingVertical(5).PaddingHorizontal(4).AlignRight().Text("NET PRİM").FontSize(7).Bold().FontColor("#ffffff");
                    header.Cell().Background("#0f3460").PaddingVertical(5).PaddingHorizontal(4).AlignCenter().Text("WoW").FontSize(7).Bold().FontColor("#ffffff");
                });
                foreach (var (item, ri) in _segment.Select((x, i) => (x, i)))
                {
                    var rowBg    = ri % 2 == 0 ? "#ffffff" : "#f8fafc";
                    var wowColor = item.WoW > 0 ? "#10b981" : item.WoW < 0 ? "#ef4444" : "#94a3b8";
                    table.Cell().Background(rowBg).BorderBottom(0.5f).BorderColor("#f1f5f9").PaddingVertical(5).PaddingHorizontal(6).Text(item.Segment).FontSize(8).Bold().FontColor("#1e293b");
                    table.Cell().Background(rowBg).BorderBottom(0.5f).BorderColor("#f1f5f9").PaddingVertical(5).PaddingHorizontal(4).AlignRight().Text($"{item.PolicyCount:N0}").FontSize(8).FontColor("#475569");
                    table.Cell().Background(rowBg).BorderBottom(0.5f).BorderColor("#f1f5f9").PaddingVertical(5).PaddingHorizontal(4).AlignRight().Text(FmtPrm(item.NetPremium)).FontSize(8).FontColor("#475569");
                    table.Cell().Background(rowBg).BorderBottom(0.5f).BorderColor("#f1f5f9").PaddingVertical(5).PaddingHorizontal(4).AlignCenter().Text(FmtWoW(item.WoW)).FontSize(8).Bold().FontColor(wowColor);
                }
            });
        });
    }

    private void ComposeAgeHeatmapSection(IContainer container)
    {
        if (!_ageHeatmap.Any()) return;
        var labels   = _ageHeatmap.Select(d => d.Label).Distinct().ToList();
        var weeks    = _ageHeatmap.Select(d => d.Week).Distinct().ToList();
        var mapPrem  = _ageHeatmap.ToDictionary(d => $"{d.Label}__{d.Week}", d => d.AvgNetPremium);
        var mapRatio = _ageHeatmap.ToDictionary(d => $"{d.Label}__{d.Week}", d => d.PolicyRatio);
        container.Column(col =>
        {
            col.Item().Element(c => SectionLabel(c, "ARAÇ YAŞ GRUBU — ORT. YAZILAN PRİM HEATMAP"));
            col.Item().Element(c => HeatmapTable(c, labels, weeks, (label, week) => mapPrem.TryGetValue($"{label}__{week}", out var v) ? v : 0, "Yaş Grubu"));

            col.Item().PaddingTop(18).Element(c => SectionLabel(c, "ARAÇ YAŞ GRUBU — POLİÇE PAYI HEATMAP"));
            col.Item().Element(c => HeatmapTableRatio(c, labels, weeks, (label, week) => mapRatio.TryGetValue($"{label}__{week}", out var v) ? v : 0, "Yaş Grubu"));
        });
    }

    private void ComposePriceHeatmapSection(IContainer container)
    {
        if (!_priceHeatmap.Any()) return;
        var labels   = _priceHeatmap.Select(d => d.Label).Distinct().ToList();
        var weeks    = _priceHeatmap.Select(d => d.Week).Distinct().ToList();
        var mapPrem  = _priceHeatmap.ToDictionary(d => $"{d.Label}__{d.Week}", d => d.AvgNetPremium);
        var mapRatio = _priceHeatmap.ToDictionary(d => $"{d.Label}__{d.Week}", d => d.PolicyRatio);
        container.Column(col =>
        {
            col.Item().Element(c => SectionLabel(c, "ARAÇ BEDELİ — ORT. YAZILAN PRİM HEATMAP"));
            col.Item().Element(c => HeatmapTable(c, labels, weeks, (label, week) => mapPrem.TryGetValue($"{label}__{week}", out var v) ? v : 0, "Araç Bedeli"));

            col.Item().PaddingTop(18).Element(c => SectionLabel(c, "ARAÇ BEDELİ — POLİÇE PAYI HEATMAP"));
            col.Item().Element(c => HeatmapTableRatio(c, labels, weeks, (label, week) => mapRatio.TryGetValue($"{label}__{week}", out var v) ? v : 0, "Araç Bedeli"));
        });
    }
}
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using DataAnalysis.Application.Features.Agency.Queries.GetAgencyKpi;
using DataAnalysis.Application.Features.Agency.Queries.GetAgencyList;
using DataAnalysis.Application.Features.Agency.Queries.GetAgencyTrend;
using DataAnalysis.Application.Features.Agency.Queries.GetAgencyRegion;
using DataAnalysis.Application.Features.Agency.Queries.GetAgencyProfile;
using DataAnalysis.Application.Features.Agency.Queries.GetAgencyHeatmap;

namespace DataAnalysis.Infrastructure.Pdf;

public class AgencyPdfReport : BasePdfReport
{
    private readonly GetAgencyKpiResponse _kpi;
    private readonly List<GetAgencyListResponse> _list;
    private readonly List<GetAgencyTrendResponse> _trend;
    private readonly List<GetAgencyRegionResponse> _region;
    private readonly GetAgencyProfileResponse _profile;
    private readonly List<GetAgencyHeatmapResponse> _heatmap;
    private readonly string _selectedAgencyName;
    private readonly string _topRegion;
    private readonly GetAgencyListResponse? _regionAgencies;

    private static readonly List<string> ChartColors =
    [
        "#10B981", "#3B82F6", "#F59E0B", "#EF4444", "#8B5CF6",
        "#06B6D4", "#F97316", "#EC4899", "#84CC16"
    ];

    public AgencyPdfReport(
        GetAgencyKpiResponse kpi,
        List<GetAgencyListResponse> list,
        List<GetAgencyTrendResponse> trend,
        List<GetAgencyRegionResponse> region,
        GetAgencyProfileResponse profile,
        List<GetAgencyHeatmapResponse> heatmap,
       string productGroup,
        string dateRange,
        string selectedAgencyName = "",
        string topRegion = "",
        GetAgencyListResponse? regionAgencies = null,
        string filterSummary = "")
        : base("Acente Analizi Raporu", productGroup, dateRange, filterSummary)
    {
        _kpi     = kpi;
        _list    = list;
        _trend   = trend;
        _region  = region;
        _profile = profile;
        _heatmap = heatmap;
        _selectedAgencyName = selectedAgencyName;
        _topRegion = topRegion;
        _regionAgencies = regionAgencies;
    }

    protected override void ComposeContent(IContainer container)
    {
        container.Column(col =>
        {
            col.Spacing(18);
            
            // Sayfa 1: KPI + Top 50 Acente Listesi
            col.Item().Element(ComposeKpiSection);
            col.Item().Element(ComposeListSection);
            
            // Sayfa 2: Trend Grafik + Tablo + Profil
            col.Item().PageBreak();
            col.Item().Element(ComposeTrendSection);
            col.Item().Element(ComposeProfileSection);
            
            // Sayfa 3: Bölge Dağılımı (Sol: Özet Kart, Sağ: Top 10 Acente)
            col.Item().PageBreak();
            col.Item().Element(ComposeRegionSection);
            
            // Sayfa 4: Heatmap (Top 50)
            col.Item().PageBreak();
            col.Item().Element(ComposeHeatmapSection);
        });
    }

    #region KPI Section - 4 Kart (Paneldeki gibi)
    private void ComposeKpiSection(IContainer container)
    {
        container.Column(col =>
        {
            col.Item().Element(c => SectionLabel(c, "ACENTE KPI"));

            col.Item().Row(row =>
            {
                // 1. En Yüksek Primli Acente
                row.RelativeItem().Element(c => AgencyKpiCard(c,
                    "EN YÜKSEK PRİMLİ ACENTE", "#10b981",
                    _kpi.TopPremiumAgency,
                    FmtPrm(_kpi.TopPremiumAmount),
                    GetLastWeekRange(),
                    _kpi.PrevTopPremiumAgency ?? "—",
                    FmtPrm(_kpi.PrevTopPremiumAmount),
                    GetPrevWeekRange()));

                row.ConstantItem(10);

                // 2. En Yüksek Ort. Primli
                row.RelativeItem().Element(c => AgencyKpiCard(c,
                    "EN YÜKSEK ORT. PRİMLİ", "#3b82f6",
                    _kpi.TopAvgPremiumAgency,
                    FmtPrm(_kpi.TopAvgPremiumAmount),
                    GetLastWeekRange(),
                    _kpi.PrevTopAvgPremiumAgency ?? "—",
                    FmtPrm(_kpi.PrevTopAvgPremiumAmount),
                    GetPrevWeekRange()));

                row.ConstantItem(10);

                // 3. Aktif Acente Sayısı
                row.RelativeItem().Element(c => StatKpiCard(c,
                    "AKTİF ACENTE SAYISI", "#f59e0b",
                    _kpi.ActiveAgencyCount.ToString("N0"),
                    _kpi.ActiveAgencyCountWoW,
                    GetLastWeekRange(),
                    _kpi.PrevActiveAgencyCount.ToString("N0"),
                    _kpi.PrevActiveAgencyCountWoW,
                    GetPrevWeekRange()));

                row.ConstantItem(10);

                // 4. Ort. Prim / Acente
                row.RelativeItem().Element(c => StatKpiCard(c,
                    "ORT. PRİM / ACENTE", "#8b5cf6",
                    FmtPrm(_kpi.AvgPremiumPerAgency),
                    _kpi.AvgPremiumPerAgencyWoW,
                    GetLastWeekRange(),
                    FmtPrm(_kpi.PrevAvgPremiumPerAgency),
                    _kpi.PrevAvgPremiumPerAgencyWoW,
                    GetPrevWeekRange()));
            });
        });
    }

    private static void AgencyKpiCard(IContainer container,
        string label, string accentColor,
        string agencyName, string value, string weekRange,
        string prevAgencyName, string prevValue, string prevRange)
    {
        container
            .Border(0.5f).BorderColor("#e2e8f0")
            .BorderTop(2f).BorderColor(accentColor)
            .Padding(11)
            .Column(col =>
            {
                col.Item().Text(label).FontSize(7).Bold().FontColor("#64748b").LetterSpacing(0.05f);
                col.Item().PaddingTop(5).Text(agencyName).FontSize(10).FontColor("#475569");
                col.Item().PaddingTop(3).Text(value).FontSize(18).Bold().FontColor("#0f172a");
                col.Item().PaddingTop(3).Text(weekRange).FontSize(7).FontColor("#94a3b8");
                col.Item().PaddingTop(8).BorderTop(0.5f).BorderColor("#f1f5f9").PaddingTop(7).Column(prev =>
                {
                    prev.Item().Text(prevRange).FontSize(7).FontColor("#94a3b8");
                    prev.Item().PaddingTop(2).Text(prevAgencyName).FontSize(8).FontColor("#64748b");
                    prev.Item().PaddingTop(1).Text(prevValue).FontSize(9).Bold().FontColor("#475569");
                });
            });
    }

    private static void StatKpiCard(IContainer container,
        string label, string accentColor,
        string value, decimal wow, string weekRange,
        string prevValue, decimal prevWow, string prevRange)
    {
        var wowColor = wow >= 0 ? "#10b981" : "#ef4444";
        var prevWowColor = prevWow >= 0 ? "#10b981" : "#ef4444";

        container
            .Border(0.5f).BorderColor("#e2e8f0")
            .BorderTop(2f).BorderColor(accentColor)
            .Padding(11)
            .Column(col =>
            {
                col.Item().Text(label).FontSize(7).Bold().FontColor("#64748b").LetterSpacing(0.05f);
                col.Item().PaddingTop(7).Text(value).FontSize(20).Bold().FontColor("#0f172a");
                col.Item().PaddingTop(2).Text(FmtWoW(wow)).FontSize(10).Bold().FontColor(wowColor);
                col.Item().PaddingTop(3).Text(weekRange).FontSize(7).FontColor("#94a3b8");
                col.Item().PaddingTop(8).BorderTop(0.5f).BorderColor("#f1f5f9").PaddingTop(7).Column(prev =>
                {
                    prev.Item().Text(prevRange).FontSize(7).FontColor("#94a3b8");
                    prev.Item().PaddingTop(2).Row(r =>
                    {
                        r.AutoItem().Text(prevValue).FontSize(8).FontColor("#475569");
                        r.ConstantItem(6);
                        r.AutoItem().Text(FmtWoW(prevWow)).FontSize(8).Bold().FontColor(prevWowColor);
                    });
                });
            });
    }
    #endregion

    #region List Section - Top 50 Acente
    private void ComposeListSection(IContainer container)
    {
        var items = _list.SelectMany(l => l.Items).ToList();
        if (!items.Any()) return;

        container.Column(col =>
        {
            col.Item().Element(c => SectionLabel(c, "GENEL TOP 50 ACENTE (NET PRİM)"));

            col.Item()
                .Border(0.5f).BorderColor("#e2e8f0")
                .Padding(12)
                .Table(table =>
                {
                    table.ColumnsDefinition(cols =>
                    {
                        cols.ConstantColumn(22);    // #
                        cols.RelativeColumn(3f);    // Acente Adı
                        cols.RelativeColumn();      // Poliçe
                        cols.RelativeColumn(1.3f);  // Net Prim
                        cols.RelativeColumn(1.2f);  // Ort Prim
                        cols.ConstantColumn(44);    // WoW
                    });

                    table.Header(header =>
                    {
                        header.Cell().Background("#0f3460").PaddingVertical(5).PaddingHorizontal(4).AlignCenter()
                            .Text("#").FontSize(7).Bold().FontColor("#ffffff");
                        header.Cell().Background("#0f3460").PaddingVertical(5).PaddingHorizontal(6)
                            .Text("ACENTE").FontSize(7).Bold().FontColor("#ffffff");
                        header.Cell().Background("#0f3460").PaddingVertical(5).PaddingHorizontal(4).AlignRight()
                            .Text("POLİÇE").FontSize(7).Bold().FontColor("#ffffff");
                        header.Cell().Background("#0f3460").PaddingVertical(5).PaddingHorizontal(4).AlignRight()
                            .Text("NET PRİM").FontSize(7).Bold().FontColor("#ffffff");
                        header.Cell().Background("#0f3460").PaddingVertical(5).PaddingHorizontal(4).AlignRight()
                            .Text("ORT. PRİM").FontSize(7).Bold().FontColor("#ffffff");
                        header.Cell().Background("#0f3460").PaddingVertical(5).PaddingHorizontal(4).AlignCenter()
                            .Text("WoW").FontSize(7).Bold().FontColor("#ffffff");
                    });

                    foreach (var (item, ri) in items.Take(50).Select((x, i) => (x, i)))
                    {
                        var rowBg    = ri % 2 == 0 ? "#ffffff" : "#f8fafc";
                        var wowColor = item.WowChange > 0 ? "#10b981" : item.WowChange < 0 ? "#ef4444" : "#94a3b8";

                        table.Cell().Background(rowBg).BorderBottom(0.5f).BorderColor("#f1f5f9")
                            .PaddingVertical(4).PaddingHorizontal(4).AlignCenter()
                            .Text($"{ri + 1}").FontSize(7).FontColor("#94a3b8");

                        table.Cell().Background(rowBg).BorderBottom(0.5f).BorderColor("#f1f5f9")
                            .PaddingVertical(4).PaddingHorizontal(6)
                            .Text(item.AgencyName).FontSize(7.5f).Bold().FontColor("#1e293b");

                        table.Cell().Background(rowBg).BorderBottom(0.5f).BorderColor("#f1f5f9")
                            .PaddingVertical(4).PaddingHorizontal(4).AlignRight()
                            .Text($"{item.PolicyCount:N0}").FontSize(7.5f).FontColor("#475569");

                        table.Cell().Background(rowBg).BorderBottom(0.5f).BorderColor("#f1f5f9")
                            .PaddingVertical(4).PaddingHorizontal(4).AlignRight()
                            .Text(FmtPrm(item.NetPremium)).FontSize(7.5f).FontColor("#0f172a");

                        table.Cell().Background(rowBg).BorderBottom(0.5f).BorderColor("#f1f5f9")
                            .PaddingVertical(4).PaddingHorizontal(4).AlignRight()
                            .Text(FmtPrm(item.AvgPremium)).FontSize(7.5f).FontColor("#475569");

                        table.Cell().Background(rowBg).BorderBottom(0.5f).BorderColor("#f1f5f9")
                            .PaddingVertical(4).PaddingHorizontal(4).AlignCenter()
                            .Text(FmtWoW(item.WowChange)).FontSize(7.5f).Bold().FontColor(wowColor);
                    }
                });
        });
    }
    #endregion

    #region Trend Section - Grafik + Tablo (Paneldeki gibi)
    private void ComposeTrendSection(IContainer container)
    {
        var items = _trend.SelectMany(t => t.Items).ToList();
        if (!items.Any()) return;

        var weeks = items.Select(i => i.Week).ToList();
        var agencyName = !string.IsNullOrEmpty(_selectedAgencyName) ? _selectedAgencyName : _kpi.TopPremiumAgency;

        // Özet hesaplamaları
        var totalPrem  = items.Sum(i => i.NetPremium);
        var totalCount = items.Sum(i => i.PolicyCount);
        var avgPremium = totalCount > 0 ? totalPrem / totalCount : 0;

        container.Column(col =>
        {
            col.Item().Element(c => SectionLabel(c, $"ACENTE TREND & PROFİL — {agencyName}"));

            col.Item().Row(row =>
            {
                // SOL: Grafik + Özet (2/5)
                row.RelativeItem(2).Column(left =>
                {
                    // Grafik
                    left.Item().Border(0.5f).BorderColor("#e2e8f0").Padding(10).Column(chart =>
                    {
                        var svgContent = DrawTrendChart(items, 380, 130);
                        chart.Item().Svg(svgContent);
                    });

                    left.Item().PaddingTop(8);

                    // Özet Satırı (Toplam Prim | Poliçe | Ort. Prim)
                    left.Item().Border(0.5f).BorderColor("#e2e8f0").Row(stats =>
                    {
                        stats.RelativeItem().BorderRight(0.5f).BorderColor("#e2e8f0").Padding(8).Column(c =>
                        {
                            c.Item().Text("TOPLAM PRİM").FontSize(6).Bold().FontColor("#94a3b8");
                            c.Item().PaddingTop(3).Text(FmtPrm(totalPrem)).FontSize(11).Bold().FontColor("#0f172a");
                        });
                        stats.RelativeItem().BorderRight(0.5f).BorderColor("#e2e8f0").Padding(8).Column(c =>
                        {
                            c.Item().Text("POLİÇE").FontSize(6).Bold().FontColor("#94a3b8");
                            c.Item().PaddingTop(3).Text($"{totalCount:N0}").FontSize(11).Bold().FontColor("#0f172a");
                        });
                        stats.RelativeItem().Padding(8).Column(c =>
                        {
                            c.Item().Text("ORT. PRİM").FontSize(6).Bold().FontColor("#94a3b8");
                            c.Item().PaddingTop(3).Text(FmtPrm(avgPremium)).FontSize(11).Bold().FontColor("#0f172a");
                        });
                    });
                });

                row.ConstantItem(10);

                // SAĞ: Haftalık Tablo (3/5)
                row.RelativeItem(3).Border(0.5f).BorderColor("#e2e8f0").Padding(10)
                    .Table(table =>
                    {
                        table.ColumnsDefinition(cols =>
                        {
                            cols.RelativeColumn(1.5f);  // Hafta
                            cols.RelativeColumn();      // Poliçe
                            cols.RelativeColumn(1.5f);  // Net Prim
                            cols.RelativeColumn(1.3f);  // Ort Prim
                        });

                        table.Header(header =>
                        {
                            header.Cell().Background("#0f3460").PaddingVertical(5).PaddingHorizontal(6)
                                .Text("HAFTA").FontSize(7).Bold().FontColor("#ffffff");
                            header.Cell().Background("#0f3460").PaddingVertical(5).PaddingHorizontal(4).AlignRight()
                                .Text("POLİÇE").FontSize(7).Bold().FontColor("#ffffff");
                            header.Cell().Background("#0f3460").PaddingVertical(5).PaddingHorizontal(4).AlignRight()
                                .Text("NET PRİM").FontSize(7).Bold().FontColor("#ffffff");
                            header.Cell().Background("#0f3460").PaddingVertical(5).PaddingHorizontal(4).AlignRight()
                                .Text("ORT. PRİM").FontSize(7).Bold().FontColor("#ffffff");
                        });

                        var lastWeek = items.LastOrDefault()?.Week ?? "";
                        foreach (var (item, ri) in items.Select((x, i) => (x, i)))
                        {
                            var rowBg  = ri % 2 == 0 ? "#ffffff" : "#f8fafc";
                            var isLast = item.Week == lastWeek;

                            table.Cell().Background(rowBg).BorderBottom(0.5f).BorderColor("#f1f5f9")
                                .PaddingVertical(5).PaddingHorizontal(6)
                                .Text(item.Week).FontSize(isLast ? 8 : 7.5f).FontColor("#1e293b");

                            table.Cell().Background(rowBg).BorderBottom(0.5f).BorderColor("#f1f5f9")
                                .PaddingVertical(5).PaddingHorizontal(4).AlignRight()
                                .Text($"{item.PolicyCount:N0}").FontSize(isLast ? 8 : 7.5f).FontColor(isLast ? "#0f172a" : "#475569");

                            table.Cell().Background(rowBg).BorderBottom(0.5f).BorderColor("#f1f5f9")
                                .PaddingVertical(5).PaddingHorizontal(4).AlignRight()
                                .Text(FmtPrm(item.NetPremium)).FontSize(isLast ? 8 : 7.5f).FontColor(isLast ? "#0f172a" : "#475569");

                            table.Cell().Background(rowBg).BorderBottom(0.5f).BorderColor("#f1f5f9")
                                .PaddingVertical(5).PaddingHorizontal(4).AlignRight()
                                .Text(FmtPrm(item.AvgPremium)).FontSize(isLast ? 8 : 7.5f).FontColor(isLast ? "#0f172a" : "#475569");
                        }
                    });
            });
        });
    }

    private string DrawTrendChart(List<AgencyTrendItem> items, int width = 380, int height = 130)
    {
        const int paddingLeft   = 50;
        const int paddingRight  = 10;
        const int paddingTop    = 10;
        const int paddingBottom = 24;

        var chartW = width  - paddingLeft - paddingRight;
        var chartH = height - paddingTop  - paddingBottom;

        var values = items.Select(i => i.NetPremium).ToList();
        if (!values.Any()) return $"<svg width='{width}' height='{height}' xmlns='http://www.w3.org/2000/svg'></svg>";

        var minVal = 0m;
        var maxVal = values.Max() * 1.1m;
        var range  = maxVal - minVal;
        if (range == 0) range = 1;

        double X(int wi) => paddingLeft + wi * (chartW / (double)Math.Max(items.Count - 1, 1));
        double Y(decimal v) => paddingTop + chartH - (double)((v - minVal) / range) * chartH;

        var sb = new System.Text.StringBuilder();
        sb.Append($"<svg width='{width}' height='{height}' xmlns='http://www.w3.org/2000/svg'>");

        // Grid lines
        for (int i = 0; i <= 4; i++)
        {
            var y   = paddingTop + i * (chartH / 4.0);
            var val = maxVal - (range / 4) * i;
            var label = val >= 1_000_000 ? $"₺{(val / 1_000_000m):F1}M" : $"₺{(val / 1_000m):F0}K";
            sb.Append($"<line x1='{paddingLeft}' y1='{y:F1}' x2='{width - paddingRight}' y2='{y:F1}' stroke='#f1f5f9' stroke-width='0.5'/>");
            sb.Append($"<text x='{paddingLeft - 4}' y='{y + 3:F1}' text-anchor='end' font-size='7' font-family='Arial' fill='#94a3b8'>{label}</text>");
        }

        // X axis labels
        for (int wi = 0; wi < items.Count; wi++)
        {
            var x = X(wi);
            sb.Append($"<text x='{x:F1}' y='{height - 4}' text-anchor='middle' font-size='6.5' font-family='Arial' fill='#94a3b8'>{items[wi].Week}</text>");
        }

        // Line
        var points = new List<string>();
        for (int wi = 0; wi < items.Count; wi++)
        {
            points.Add($"{X(wi):F1},{Y(items[wi].NetPremium):F1}");
        }
        sb.Append($"<polyline points='{string.Join(" ", points)}' fill='none' stroke='#10b981' stroke-width='2' stroke-linejoin='round' stroke-linecap='round'/>");

        // Dots
        for (int wi = 0; wi < items.Count; wi++)
        {
            var px = X(wi);
            var py = Y(items[wi].NetPremium);
            sb.Append($"<circle cx='{px:F1}' cy='{py:F1}' r='3' fill='#10b981' stroke='white' stroke-width='1.5'/>");
        }

        sb.Append("</svg>");
        return sb.ToString();
    }
    #endregion

    #region Profile Section - 4 Kutu (Paneldeki gibi)
    private void ComposeProfileSection(IContainer container)
    {
        if (_profile == null) return;

        container.Column(col =>
        {
            col.Item().Row(row =>
            {
                // Top 3 Marka
                row.RelativeItem().Border(0.5f).BorderColor("#e2e8f0").Padding(10).Column(inner =>
                {
                    inner.Item().PaddingBottom(8).Text("TOP 3 MARKA")
                        .FontSize(7).Bold().FontColor("#64748b").LetterSpacing(0.05f);

                    foreach (var (brand, ri) in _profile.TopBrands.Take(3).Select((b, i) => (b, i)))
                    {
                        var rowBg = ri % 2 == 0 ? "#ffffff" : "#f8fafc";
                        inner.Item().Background(rowBg).BorderBottom(0.5f).BorderColor("#f1f5f9")
                            .PaddingVertical(5).Row(r =>
                            {
                                r.AutoItem().Text($"{ri + 1}.").FontSize(8).Bold().FontColor("#94a3b8");
                                r.ConstantItem(4);
                                r.RelativeItem().Text(brand.Brand).FontSize(8).Bold().FontColor("#1e293b");
                                r.AutoItem().Text($"{brand.PolicyCount:N0}").FontSize(8).FontColor("#475569");
                            });
                    }
                });

                row.ConstantItem(10);

                // Yenileme Tipi
                row.RelativeItem().Border(0.5f).BorderColor("#e2e8f0").Padding(10).Column(inner =>
                {
                    inner.Item().PaddingBottom(8).Text("YENİLEME TİPİ")
                        .FontSize(7).Bold().FontColor("#64748b").LetterSpacing(0.05f);

                    foreach (var (item, ri) in _profile.YenilemeTipi.Select((x, i) => (x, i)))
                    {
                        var rowBg = ri % 2 == 0 ? "#ffffff" : "#f8fafc";
                        inner.Item().Background(rowBg).BorderBottom(0.5f).BorderColor("#f1f5f9")
                            .PaddingVertical(5).Row(r =>
                            {
                                r.RelativeItem().Text(item.Type).FontSize(8).Bold().FontColor("#1e293b");
                                r.AutoItem().Text($"%{item.Ratio:F1}").FontSize(8).FontColor("#8b5cf6");
                            });
                    }
                });

                row.ConstantItem(10);

                // Sigortalı Türü
                row.RelativeItem().Border(0.5f).BorderColor("#e2e8f0").Padding(10).Column(inner =>
                {
                    inner.Item().PaddingBottom(8).Text("SİGORTALI TÜRÜ")
                        .FontSize(7).Bold().FontColor("#64748b").LetterSpacing(0.05f);

                    foreach (var (item, ri) in _profile.SigortaliTuru.Select((x, i) => (x, i)))
                    {
                        var rowBg = ri % 2 == 0 ? "#ffffff" : "#f8fafc";
                        inner.Item().Background(rowBg).BorderBottom(0.5f).BorderColor("#f1f5f9")
                            .PaddingVertical(5).Row(r =>
                            {
                                r.RelativeItem().Text(item.Type).FontSize(8).Bold().FontColor("#1e293b");
                                r.AutoItem().Text($"%{item.Ratio:F1}").FontSize(8).FontColor("#10b981");
                            });
                    }
                });

                row.ConstantItem(10);

                // Basamak Dağılımı
                row.RelativeItem().Border(0.5f).BorderColor("#e2e8f0").Padding(10).Column(inner =>
                {
                    inner.Item().PaddingBottom(8).Text("BASAMAK DAĞILIMI")
                        .FontSize(7).Bold().FontColor("#64748b").LetterSpacing(0.05f);

                    foreach (var (item, ri) in _profile.Basamak.Select((x, i) => (x, i)))
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
    #endregion

    #region Region Section - Sol: Bölge Özet Kartı, Sağ: Top 10 Acente (Paneldeki gibi)
    private void ComposeRegionSection(IContainer container)
    {
        var regionItems = _region.SelectMany(r => r.Items).ToList();
        if (!regionItems.Any()) return;

        // En yüksek primli bölgeyi bul
        var topRegionData = regionItems.OrderByDescending(r => r.NetPremium).FirstOrDefault();
        if (topRegionData == null) return;

        var regionName = !string.IsNullOrEmpty(_topRegion) ? _topRegion : topRegionData.Region;
        var currentRegion = regionItems.FirstOrDefault(r => r.Region == regionName) ?? topRegionData;
        
        var avgPremium = currentRegion.PolicyCount > 0 ? currentRegion.NetPremium / currentRegion.PolicyCount : 0;
        var totalPolicies = regionItems.Sum(r => r.PolicyCount);
        var policyPercentage = totalPolicies > 0 ? (decimal)currentRegion.PolicyCount / totalPolicies * 100 : 0;
        var wowChange = currentRegion.WowChange;
        var isPositive = wowChange >= 0;

        container.Column(col =>
        {
            col.Item().Element(c => SectionLabel(c, "BÖLGE DAĞILIMI"));

            col.Item().Row(row =>
            {
                // SOL: Bölge Özet Kartı (1/3)
                row.RelativeItem().Border(0.5f).BorderColor("#e2e8f0").Padding(12).Column(card =>
                {
                    // Bölge Başlığı + WoW
                    card.Item().Row(header =>
                    {
                        header.RelativeItem().Column(title =>
                        {
                            title.Item().Text(currentRegion.Region).FontSize(14).Bold().FontColor("#0f172a");
                            title.Item().PaddingTop(2).Text(FmtWoW(wowChange))
                                .FontSize(10).Bold().FontColor(isPositive ? "#10b981" : "#ef4444");
                        });
                    });

                    card.Item().PaddingTop(12).BorderTop(0.5f).BorderColor("#f1f5f9").PaddingTop(10);

                    // Toplam Prim
                    card.Item().Column(stat =>
                    {
                        stat.Item().Text("TOPLAM PRİM").FontSize(7).Bold().FontColor("#94a3b8");
                        stat.Item().PaddingTop(3).Text(FmtPrm(currentRegion.NetPremium)).FontSize(18).Bold().FontColor("#0f172a");
                    });

                    card.Item().PaddingTop(10);

                    // Ort. Prim
                    card.Item().Column(stat =>
                    {
                        stat.Item().Text("ORT. PRİM").FontSize(7).Bold().FontColor("#94a3b8");
                        stat.Item().PaddingTop(3).Text(FmtPrm(avgPremium)).FontSize(16).Bold().FontColor("#475569");
                    });

                    card.Item().PaddingTop(10);

                    // Prim Payı ve Poliçe yan yana
                    card.Item().Row(stats =>
                    {
                        // Prim Payı
                        stats.RelativeItem().Column(c =>
                        {
                            c.Item().Text("PRİM PAYI").FontSize(7).Bold().FontColor("#94a3b8");
                            c.Item().PaddingTop(3).Text($"%{currentRegion.Ratio:F1}").FontSize(14).Bold().FontColor("#10b981");
                        });

                        // Poliçe
                        stats.RelativeItem().Column(c =>
                        {
                            c.Item().Text("POLİÇE").FontSize(7).Bold().FontColor("#94a3b8");
                            c.Item().PaddingTop(3).Text($"{currentRegion.PolicyCount:N0}").FontSize(14).Bold().FontColor("#0f172a");
                            c.Item().PaddingTop(2).Text($"(%{policyPercentage:F1} toplam)").FontSize(7).FontColor("#94a3b8");
                        });
                    });
                });

                row.ConstantItem(10);

                // SAĞ: O Bölgenin Top 10 Acentesi (2/3)
                row.RelativeItem(2).Border(0.5f).BorderColor("#e2e8f0").Padding(10).Column(tableCol =>
                {
                    tableCol.Item().PaddingBottom(8).Text($"{currentRegion.Region} — TOP 10 ACENTE")
                        .FontSize(8).Bold().FontColor("#64748b");

                    var agencies = _regionAgencies?.Items ?? [];
                    if (!agencies.Any())
                    {
                        tableCol.Item().Text("Bu bölgede acente bulunamadı.").FontSize(8).FontColor("#94a3b8");
                    }
                    else
                    {
                        tableCol.Item().Table(table =>
                        {
                            table.ColumnsDefinition(cols =>
                            {
                                cols.ConstantColumn(20);    // #
                                cols.RelativeColumn(3f);    // Acente
                                cols.RelativeColumn();      // Poliçe
                                cols.RelativeColumn(1.3f);  // Net Prim
                                cols.RelativeColumn(1.2f);  // Ort. Prim
                                cols.ConstantColumn(40);    // WoW
                            });

                            table.Header(header =>
                            {
                                header.Cell().Background("#0f3460").PaddingVertical(4).PaddingHorizontal(3).AlignCenter()
                                    .Text("#").FontSize(6.5f).Bold().FontColor("#ffffff");
                                header.Cell().Background("#0f3460").PaddingVertical(4).PaddingHorizontal(4)
                                    .Text("ACENTE").FontSize(6.5f).Bold().FontColor("#ffffff");
                                header.Cell().Background("#0f3460").PaddingVertical(4).PaddingHorizontal(3).AlignRight()
                                    .Text("POLİÇE").FontSize(6.5f).Bold().FontColor("#ffffff");
                                header.Cell().Background("#0f3460").PaddingVertical(4).PaddingHorizontal(3).AlignRight()
                                    .Text("NET PRİM").FontSize(6.5f).Bold().FontColor("#ffffff");
                                header.Cell().Background("#0f3460").PaddingVertical(4).PaddingHorizontal(3).AlignRight()
                                    .Text("ORT.").FontSize(6.5f).Bold().FontColor("#ffffff");
                                header.Cell().Background("#0f3460").PaddingVertical(4).PaddingHorizontal(3).AlignCenter()
                                    .Text("WoW").FontSize(6.5f).Bold().FontColor("#ffffff");
                            });

                            foreach (var (item, ri) in agencies.Take(10).Select((x, i) => (x, i)))
                            {
                                var rowBg    = ri % 2 == 0 ? "#ffffff" : "#f8fafc";
                                var wowColor = item.WowChange > 0 ? "#10b981" : item.WowChange < 0 ? "#ef4444" : "#94a3b8";

                                table.Cell().Background(rowBg).BorderBottom(0.5f).BorderColor("#f1f5f9")
                                    .PaddingVertical(4).PaddingHorizontal(3).AlignCenter()
                                    .Text($"{ri + 1}").FontSize(7).FontColor("#94a3b8");

                                table.Cell().Background(rowBg).BorderBottom(0.5f).BorderColor("#f1f5f9")
                                    .PaddingVertical(4).PaddingHorizontal(4)
                                    .Text(item.AgencyName).FontSize(7).Bold().FontColor("#1e293b");

                                table.Cell().Background(rowBg).BorderBottom(0.5f).BorderColor("#f1f5f9")
                                    .PaddingVertical(4).PaddingHorizontal(3).AlignRight()
                                    .Text($"{item.PolicyCount:N0}").FontSize(7).FontColor("#475569");

                                table.Cell().Background(rowBg).BorderBottom(0.5f).BorderColor("#f1f5f9")
                                    .PaddingVertical(4).PaddingHorizontal(3).AlignRight()
                                    .Text(FmtPrm(item.NetPremium)).FontSize(7).FontColor("#0f172a");

                                table.Cell().Background(rowBg).BorderBottom(0.5f).BorderColor("#f1f5f9")
                                    .PaddingVertical(4).PaddingHorizontal(3).AlignRight()
                                    .Text(FmtPrm(item.AvgPremium)).FontSize(7).FontColor("#475569");

                                table.Cell().Background(rowBg).BorderBottom(0.5f).BorderColor("#f1f5f9")
                                    .PaddingVertical(4).PaddingHorizontal(3).AlignCenter()
                                    .Text(FmtWoW(item.WowChange)).FontSize(7).Bold().FontColor(wowColor);
                            }
                        });
                    }
                });
            });
        });
    }
    #endregion

    #region Heatmap Section - Top 50 Acente
    private void ComposeHeatmapSection(IContainer container)
    {
        var heatmapData = _heatmap.FirstOrDefault();
        if (heatmapData == null || !heatmapData.Rows.Any()) return;

        var agencies = heatmapData.Rows.Take(50).Select(r => r.AgencyName).ToList();
        var weeks    = heatmapData.Weeks;

        // Build value maps
        var premMap  = new Dictionary<string, decimal>();
        var ratioMap = new Dictionary<string, decimal>();
        foreach (var row in heatmapData.Rows.Take(50))
        {
            foreach (var cell in row.Cells)
            {
                premMap[$"{row.AgencyName}__{cell.Week}"]  = cell.NetPremium;
                ratioMap[$"{row.AgencyName}__{cell.Week}"] = cell.PolicyRatio;
            }
        }

        container.Column(col =>
        {
            // Ort. Yazılan Prim Heatmap
            col.Item().Element(c => SectionLabel(c, "ORT. YAZILAN PRİM HEATMAP — TOP 50"));
            col.Item().Element(c => HeatmapTable(c, agencies, weeks,
                (agency, week) => premMap.TryGetValue($"{agency}__{week}", out var v) ? v : 0,
                "ACENTE"));

            // Poliçe Payı Heatmap
            col.Item().PaddingTop(18).Element(c => SectionLabel(c, "POLİÇE PAYI HEATMAP — TOP 50"));
            col.Item().Element(c => HeatmapTableRatio(c, agencies, weeks,
                (agency, week) => ratioMap.TryGetValue($"{agency}__{week}", out var v) ? v : 0,
                "ACENTE"));
        });
    }
    #endregion
}
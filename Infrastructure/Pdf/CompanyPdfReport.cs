using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using DataAnalysis.Application.Features.Company.Queries.GetCompanyKpi;
using DataAnalysis.Application.Features.Company.Queries.GetCompanyList;
using DataAnalysis.Application.Features.Company.Queries.GetCompanyTrend;
using DataAnalysis.Application.Features.Company.Queries.GetCompanyRenewal;
using DataAnalysis.Application.Features.Company.Queries.GetCompanyProfile;
using DataAnalysis.Application.Features.Company.Queries.GetCompanyHeatmap;

namespace DataAnalysis.Infrastructure.Pdf;

public class CompanyPdfReport : BasePdfReport
{
    private readonly GetCompanyKpiResponse _kpi;
    private readonly List<GetCompanyListResponse> _list;
    private readonly List<GetCompanyTrendResponse> _trend;
    private readonly List<GetCompanyRenewalResponse> _renewal;
    private readonly GetCompanyProfileResponse _profile;
    private readonly List<GetCompanyHeatmapResponse> _heatmap;

    private static readonly List<string> ChartColors =
    [
        "#10B981", "#3B82F6", "#F59E0B", "#EF4444", "#8B5CF6",
        "#06B6D4", "#F97316", "#EC4899", "#84CC16"
    ];

    public CompanyPdfReport(
        GetCompanyKpiResponse kpi,
        List<GetCompanyListResponse> list,
        List<GetCompanyTrendResponse> trend,
        List<GetCompanyRenewalResponse> renewal,
        GetCompanyProfileResponse profile,
        List<GetCompanyHeatmapResponse> heatmap,
        string productGroup,
        string dateRange,
        string filterSummary = "")
        : base("Şirket Geçiş Analizi Raporu", productGroup, dateRange, filterSummary)
    {
        _kpi     = kpi;
        _list    = list;
        _trend   = trend;
        _renewal = renewal;
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
            col.Item().Element(ComposeRenewalTrendSection);
            col.Item().Element(ComposeTrendSection);
            col.Item().Element(ComposeProfileSection);
            col.Item().PageBreak();
            col.Item().Element(ComposeHeatmapSection);
        });
    }

    private void ComposeKpiSection(IContainer container)
    {
        container.Column(col =>
        {
            col.Item().Element(c => SectionLabel(c, "ŞİRKET GEÇİŞ KPI"));

            col.Item().Row(row =>
            {
                row.RelativeItem().Element(c => KpiCard(c,
                    "EN ÇOK GEÇİŞ GELEN", "#10b981",
                    _kpi.TopCompanyByCount, $"{_kpi.TopCompanyCount:N0} Poliçe", GetLastWeekRange(),
                    _kpi.PrevTopCompanyByCount, $"{_kpi.PrevTopCompanyCount:N0} Poliçe", GetPrevWeekRange()));

                row.ConstantItem(10);

                row.RelativeItem().Element(c => KpiCard(c,
                    "EN ÇOK PRİM GELEN", "#3b82f6",
                    _kpi.TopCompanyByPremium, FmtPrm(_kpi.TopCompanyPremium), GetLastWeekRange(),
                    _kpi.PrevTopCompanyByPremium, FmtPrm(_kpi.PrevTopCompanyPremium), GetPrevWeekRange()));

                row.ConstantItem(10);

                row.RelativeItem().Element(c => RatioKpiCard(c,
                    "YENİ İŞ ORANI", "#f59e0b",
                    $"%{_kpi.NewBusinessRatio:F1}", FmtPp(_kpi.NewBusinessRatioWoW),
                    _kpi.NewBusinessRatioWoW >= 0 ? "#10b981" : "#ef4444",
                    GetLastWeekRange(), $"%{_kpi.PrevNewBusinessRatio:F1}", GetPrevWeekRange()));

                row.ConstantItem(10);

                row.RelativeItem().Element(c => RatioKpiCard(c,
                    "TRANSFER ORANI", "#8b5cf6",
                    $"%{_kpi.RenewalRatio:F1}", FmtPp(_kpi.RenewalRatioWoW),
                    _kpi.RenewalRatioWoW >= 0 ? "#10b981" : "#ef4444",
                    GetLastWeekRange(), $"%{_kpi.PrevRenewalRatio:F1}", GetPrevWeekRange()));
            });
        });
    }

    private static void RatioKpiCard(IContainer container,
        string label, string accentColor,
        string value, string changeValue, string changeColor,
        string weekRange, string prevValue, string prevRange)
    {
        container
            .Border(0.5f).BorderColor("#e2e8f0")
            .BorderTop(2f).BorderColor(accentColor)
            .Padding(11)
            .Column(col =>
            {
                col.Item().Text(label).FontSize(7).Bold().FontColor("#64748b").LetterSpacing(0.05f);
                col.Item().PaddingTop(7).Row(r =>
                {
                    r.AutoItem().Text(value).FontSize(18).Bold().FontColor("#0f172a");
                    r.ConstantItem(8);
                    r.AutoItem().AlignBottom().PaddingBottom(2).Text(changeValue).FontSize(10).Bold().FontColor(changeColor);
                });
                col.Item().PaddingTop(3).Text(weekRange).FontSize(7).FontColor("#94a3b8");
                col.Item().PaddingTop(8).BorderTop(0.5f).BorderColor("#f1f5f9").PaddingTop(7).Column(prev =>
                {
                    prev.Item().Text(prevRange).FontSize(7).FontColor("#94a3b8");
                    prev.Item().PaddingTop(2).Text(prevValue).FontSize(8).Bold().FontColor("#475569");
                });
            });
    }

    private static string FmtPp(decimal n)
        => (n >= 0 ? "+" : "") + n.ToString("F1") + "pp";

    private void ComposeListSection(IContainer container)
    {
        if (!_list.Any()) return;

        container.Column(col =>
        {
            col.Item().Element(c => SectionLabel(c, "ŞİRKET GEÇİŞ SIRALAMASI (NET PRİM)"));

            col.Item()
                .Border(0.5f).BorderColor("#e2e8f0")
                .Padding(12)
                .Table(table =>
                {
                    table.ColumnsDefinition(cols =>
                    {
                        cols.RelativeColumn(3f);
                        cols.RelativeColumn();
                        cols.RelativeColumn(1.2f);
                        cols.RelativeColumn(1.2f);
                        cols.ConstantColumn(44);
                    });

                    table.Header(header =>
                    {
                        header.Cell().Background("#0f3460").PaddingVertical(5).PaddingHorizontal(6)
                            .Text("ÖNCEKİ ŞİRKET").FontSize(7).Bold().FontColor("#ffffff");
                        header.Cell().Background("#0f3460").PaddingVertical(5).PaddingHorizontal(4).AlignRight()
                            .Text("POLİÇE").FontSize(7).Bold().FontColor("#ffffff");
                        header.Cell().Background("#0f3460").PaddingVertical(5).PaddingHorizontal(4).AlignRight()
                            .Text("NET PRİM").FontSize(7).Bold().FontColor("#ffffff");
                        header.Cell().Background("#0f3460").PaddingVertical(5).PaddingHorizontal(4).AlignRight()
                            .Text("ORT. PRİM").FontSize(7).Bold().FontColor("#ffffff");
                        header.Cell().Background("#0f3460").PaddingVertical(5).PaddingHorizontal(4).AlignCenter()
                            .Text("WoW").FontSize(7).Bold().FontColor("#ffffff");
                    });

                    foreach (var (item, ri) in _list.Take(15).Select((x, i) => (x, i)))
                    {
                        var rowBg    = ri % 2 == 0 ? "#ffffff" : "#f8fafc";
                        var wowColor = item.WoW > 0 ? "#10b981" : item.WoW < 0 ? "#ef4444" : "#94a3b8";

                        table.Cell().Background(rowBg).BorderBottom(0.5f).BorderColor("#f1f5f9")
                            .PaddingVertical(5).PaddingHorizontal(6)
                            .Text(item.Company).FontSize(8).Bold().FontColor("#1e293b");

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

    private void ComposeRenewalTrendSection(IContainer container)
    {
        if (!_renewal.Any()) return;

        var weeks = _renewal.Select(r => r.WeekLabel).ToList();

        container.Column(col =>
        {
            col.Item().Element(c => SectionLabel(c, "YENİLEME TİPİ TRENDİ (8 HAFTA)"));

            col.Item().Row(row =>
            {
                // Grafik (sol)
                row.RelativeItem(2).Border(0.5f).BorderColor("#e2e8f0").Padding(10).Column(chart =>
                {
                    // Legend - Sıralama: Yeni İş, Yenileme, Transfer
                    chart.Item().Row(legend =>
                    {
                        legend.AutoItem().Width(10).Height(10).Background("#f59e0b");
                        legend.ConstantItem(4);
                        legend.AutoItem().Text("Yeni İş").FontSize(7).FontColor("#475569");
                        legend.ConstantItem(12);
                        legend.AutoItem().Width(10).Height(10).Background("#10b981");
                        legend.ConstantItem(4);
                        legend.AutoItem().Text("Yenileme").FontSize(7).FontColor("#475569");
                        legend.ConstantItem(12);
                        legend.AutoItem().Width(10).Height(10).Background("#8b5cf6");
                        legend.ConstantItem(4);
                        legend.AutoItem().Text("Transfer").FontSize(7).FontColor("#475569");
                    });

                    var svgContent = DrawRenewalChart(weeks, 480, 140);
                    chart.Item().PaddingTop(8).Svg(svgContent);
                });

                row.ConstantItem(10);

                // Tablo (sağ) - 4 kolon: Hafta, Yeni İş, Yenileme, Transfer
                row.RelativeItem().Border(0.5f).BorderColor("#e2e8f0").Padding(10)
                    .Table(table =>
                    {
                        table.ColumnsDefinition(cols =>
                        {
                            cols.RelativeColumn(1.4f);
                            cols.RelativeColumn();
                            cols.RelativeColumn();
                            cols.RelativeColumn();
                        });

                        table.Header(header =>
                        {
                            header.Cell().Background("#0f3460").PaddingVertical(5).PaddingHorizontal(4)
                                .Text("HAFTA").FontSize(6).Bold().FontColor("#ffffff");
                            header.Cell().Background("#0f3460").PaddingVertical(5).PaddingHorizontal(3).AlignCenter()
                                .Text("YENİ İŞ").FontSize(6).Bold().FontColor("#ffffff");
                            header.Cell().Background("#0f3460").PaddingVertical(5).PaddingHorizontal(3).AlignCenter()
                                .Text("YENİLEME").FontSize(6).Bold().FontColor("#ffffff");
                            header.Cell().Background("#0f3460").PaddingVertical(5).PaddingHorizontal(3).AlignCenter()
                                .Text("TRANSFER").FontSize(6).Bold().FontColor("#ffffff");
                        });

                        var lastWeek = weeks.LastOrDefault() ?? "";
                        foreach (var (item, ri) in _renewal.Select((x, i) => (x, i)))
                        {
                            var rowBg  = ri % 2 == 0 ? "#ffffff" : "#f8fafc";
                            var isLast = item.WeekLabel == lastWeek;
                            var fontSize = isLast ? 7.5f : 7f;

                            table.Cell().Background(rowBg).BorderBottom(0.5f).BorderColor("#f1f5f9")
                                .PaddingVertical(4).PaddingHorizontal(4)
                                .Text(item.WeekLabel).FontSize(fontSize).FontColor("#1e293b");

                            table.Cell().Background(rowBg).BorderBottom(0.5f).BorderColor("#f1f5f9")
                                .PaddingVertical(4).PaddingHorizontal(3).AlignCenter()
                                .Text($"%{item.NewBusinessRatio:F1}").FontSize(fontSize).FontColor("#f59e0b");

                            table.Cell().Background(rowBg).BorderBottom(0.5f).BorderColor("#f1f5f9")
                                .PaddingVertical(4).PaddingHorizontal(3).AlignCenter()
                                .Text($"%{item.RenewalRatio:F1}").FontSize(fontSize).FontColor("#10b981");

                            table.Cell().Background(rowBg).BorderBottom(0.5f).BorderColor("#f1f5f9")
                                .PaddingVertical(4).PaddingHorizontal(3).AlignCenter()
                                .Text($"%{item.TransferRatio:F1}").FontSize(fontSize).FontColor("#8b5cf6");
                        }
                    });
            });
        });
    }

    private string DrawRenewalChart(List<string> weeks, int width = 480, int height = 140)
    {
        const int paddingLeft   = 40;
        const int paddingRight  = 8;
        const int paddingTop    = 10;
        const int paddingBottom = 24;

        var chartW = width  - paddingLeft - paddingRight;
        var chartH = height - paddingTop  - paddingBottom;

        // Max değeri 3 çizgiden hesapla
        var allVals = _renewal.SelectMany(r => new[] { r.NewBusinessRatio, r.RenewalRatio, r.TransferRatio }).ToList();
        var minVal  = 0m;
        var maxVal  = allVals.Any() ? Math.Max(allVals.Max() + 5, 70m) : 70m;
        var range   = maxVal - minVal;
        if (range == 0) range = 1;

        double X(int wi) => paddingLeft + wi * (chartW / (double)(weeks.Count - 1));
        double Y(decimal v) => paddingTop + chartH - (double)((v - minVal) / range) * chartH;

        var sb = new System.Text.StringBuilder();
        sb.Append($"<svg width='{width}' height='{height}' xmlns='http://www.w3.org/2000/svg'>");

        // Grid lines
        for (int i = 0; i <= 3; i++)
        {
            var y     = paddingTop + i * (chartH / 3.0);
            var val   = maxVal - (range / 3) * i;
            var label = $"%{val:F0}";
            sb.Append($"<line x1='{paddingLeft}' y1='{y:F1}' x2='{width - paddingRight}' y2='{y:F1}' stroke='#f1f5f9' stroke-width='0.5'/>");
            sb.Append($"<text x='{paddingLeft - 4}' y='{y + 3:F1}' text-anchor='end' font-size='7' font-family='Arial' fill='#94a3b8'>{label}</text>");
        }

        // X axis labels
        for (int wi = 0; wi < weeks.Count; wi++)
        {
            var x = X(wi);
            sb.Append($"<text x='{x:F1}' y='{height - 4}' text-anchor='middle' font-size='6.5' font-family='Arial' fill='#94a3b8'>{weeks[wi]}</text>");
        }

        // 1. Yeni İş line (amber)
        var newBizPoints = new List<string>();
        for (int wi = 0; wi < _renewal.Count; wi++)
        {
            var val = _renewal[wi].NewBusinessRatio;
            newBizPoints.Add($"{X(wi):F1},{Y(val):F1}");
        }
        sb.Append($"<polyline points='{string.Join(" ", newBizPoints)}' fill='none' stroke='#f59e0b' stroke-width='1.5' stroke-linejoin='round' stroke-linecap='round'/>");
        for (int wi = 0; wi < _renewal.Count; wi++)
            sb.Append($"<circle cx='{X(wi):F1}' cy='{Y(_renewal[wi].NewBusinessRatio):F1}' r='2.5' fill='#f59e0b' stroke='white' stroke-width='1'/>");

        // 2. Yenileme line (emerald)
        var renewalPoints = new List<string>();
        for (int wi = 0; wi < _renewal.Count; wi++)
        {
            var val = _renewal[wi].RenewalRatio;
            renewalPoints.Add($"{X(wi):F1},{Y(val):F1}");
        }
        sb.Append($"<polyline points='{string.Join(" ", renewalPoints)}' fill='none' stroke='#10b981' stroke-width='1.5' stroke-linejoin='round' stroke-linecap='round'/>");
        for (int wi = 0; wi < _renewal.Count; wi++)
            sb.Append($"<circle cx='{X(wi):F1}' cy='{Y(_renewal[wi].RenewalRatio):F1}' r='2.5' fill='#10b981' stroke='white' stroke-width='1'/>");

        // 3. Transfer line (purple)
        var transferPoints = new List<string>();
        for (int wi = 0; wi < _renewal.Count; wi++)
        {
            var val = _renewal[wi].TransferRatio;
            transferPoints.Add($"{X(wi):F1},{Y(val):F1}");
        }
        sb.Append($"<polyline points='{string.Join(" ", transferPoints)}' fill='none' stroke='#8b5cf6' stroke-width='1.5' stroke-linejoin='round' stroke-linecap='round'/>");
        for (int wi = 0; wi < _renewal.Count; wi++)
            sb.Append($"<circle cx='{X(wi):F1}' cy='{Y(_renewal[wi].TransferRatio):F1}' r='2.5' fill='#8b5cf6' stroke='white' stroke-width='1'/>");

        sb.Append("</svg>");
        return sb.ToString();
    }

    private void ComposeTrendSection(IContainer container)
    {
        if (!_trend.Any()) return;

        var company = _kpi.DefaultCompany;
        var weeks   = _trend.Select(t => t.WeekLabel).ToList();

        container.Column(col =>
        {
            col.Item().Element(c => SectionLabel(c, $"ŞİRKET TREND — {company}"));

            col.Item().Row(row =>
            {
                row.RelativeItem(2).Border(0.5f).BorderColor("#e2e8f0").Padding(10).Column(chart =>
                {
                    var svgContent = DrawLineChart(
                        new List<string> { company },
                        weeks,
                        (_, w) => _trend.FirstOrDefault(t => t.WeekLabel == w)?.NetPremium ?? 0,
                        ChartColors,
                        480, 140);
                    chart.Item().Svg(svgContent);
                });

                row.ConstantItem(10);

                row.RelativeItem().Border(0.5f).BorderColor("#e2e8f0").Padding(10)
                    .Table(table =>
                    {
                        table.ColumnsDefinition(cols =>
                        {
                            cols.RelativeColumn(1.5f);
                            cols.RelativeColumn();
                            cols.RelativeColumn();
                            cols.ConstantColumn(40);
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

                        var lastWeek = weeks.LastOrDefault() ?? "";
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
                                .Text($"{item.PolicyCount:N0}").FontSize(isLast ? 8 : 7.5f).FontColor(isLast ? "#0f172a" : "#475569");

                            table.Cell().Background(rowBg).BorderBottom(0.5f).BorderColor("#f1f5f9")
                                .PaddingVertical(5).PaddingHorizontal(4).AlignRight()
                                .Text(FmtPrm(item.NetPremium)).FontSize(isLast ? 8 : 7.5f).FontColor(isLast ? "#0f172a" : "#475569");

                            table.Cell().Background(rowBg).BorderBottom(0.5f).BorderColor("#f1f5f9")
                                .PaddingVertical(5).PaddingHorizontal(4).AlignCenter()
                                .Text(FmtWoW(item.WoW)).FontSize(8).Bold().FontColor(wowColor);
                        }
                    });
            });
        });
    }

    private void ComposeProfileSection(IContainer container)
    {
        if (_profile == null) return;

        var company = _kpi.DefaultCompany;

        var topBrands     = _profile.TopBrands;
        var sigortaliTuru = _profile.Profile.Where(x => x.Category == "SigortaliTuru").ToList();
        var aracYasi      = _profile.Profile.Where(x => x.Category == "AracYasi").OrderBy(x => x.Type).ToList();
        var basamak       = _profile.Profile.Where(x => x.Category == "Basamak").OrderBy(x => x.Type).ToList();
        var aracBedeli    = _profile.Profile.Where(x => x.Category == "AracBedeli").Take(5).ToList();

        container.Column(col =>
        {
            col.Item().Element(c => SectionLabel(c, $"GELEN MÜŞTERİ PROFİLİ — {company}"));

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
                        var ratio = total > 0 ? (float)item.PolicyCount / total * 100 : 0;
                        var rowBg = ri % 2 == 0 ? "#ffffff" : "#f8fafc";
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

        var companies = _heatmap.Select(d => d.Company).Distinct().ToList();
        var weeks     = _heatmap.Select(d => d.Week).Distinct().ToList();
        var mapPrem   = _heatmap.ToDictionary(d => $"{d.Company}__{d.Week}", d => d.AvgNetPremium);
        var mapRatio  = _heatmap.ToDictionary(d => $"{d.Company}__{d.Week}", d => d.PolicyRatio);

        container.Column(col =>
        {
            // Ort. Yazılan Prim Heatmap
            col.Item().Element(c => SectionLabel(c, "ORT. YAZILAN PRİM HEATMAP"));
            col.Item().Element(c => HeatmapTable(c, companies, weeks,
                (company, week) => mapPrem.TryGetValue($"{company}__{week}", out var v) ? v : 0,
                "ÖNCEKİ ŞİRKET"));

            // Poliçe Payı Heatmap
            col.Item().PaddingTop(18).Element(c => SectionLabel(c, "POLİÇE PAYI HEATMAP"));
            col.Item().Element(c => HeatmapTableRatio(c, companies, weeks,
                (company, week) => mapRatio.TryGetValue($"{company}__{week}", out var v) ? v : 0,
                "ÖNCEKİ ŞİRKET"));
        });
    }
}
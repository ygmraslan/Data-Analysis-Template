using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using DataAnalysis.Application.Features.Demographic.Queries.GetDemoKpi;
using DataAnalysis.Application.Features.Demographic.Queries.GetDemoDistribution;

namespace DataAnalysis.Infrastructure.Pdf;

public class DemoPdfReport : BasePdfReport
{
    private readonly GetDemoKpiResponse _kpi;
    private readonly List<GetDemoDistributionResponse> _insuredType;
    private readonly List<GetDemoDistributionResponse> _gender;
    private readonly List<GetDemoDistributionResponse> _ageGroup;
    private readonly List<GetDemoDistributionResponse> _insuredCity;

    private static readonly List<string> ChartColors =
    [
        "#10B981", "#3B82F6", "#F59E0B", "#EF4444", "#8B5CF6",
        "#06B6D4", "#F97316", "#EC4899", "#84CC16"
    ];

    public DemoPdfReport(
        GetDemoKpiResponse kpi,
        List<GetDemoDistributionResponse> insuredType,
        List<GetDemoDistributionResponse> gender,
        List<GetDemoDistributionResponse> ageGroup,
        List<GetDemoDistributionResponse> insuredCity,
        string productGroup,
        string dateRange,
        string filterSummary = "")
        : base("Demografik Analiz Raporu", productGroup, dateRange, filterSummary)
    {
        _kpi = kpi;
        _insuredType = insuredType;
        _gender = gender;
        _ageGroup = ageGroup;
        _insuredCity = insuredCity;
    }

    protected override void ComposeContent(IContainer container)
    {
        container.Column(col =>
        {
            col.Spacing(18);

            // Sayfa 1: KPI + Sigortalı Türü + Cinsiyet (alt alta)
            col.Item().Element(ComposeKpiSection);
            col.Item().Element(ComposeInsuredTypeSection);
            col.Item().Element(ComposeGenderSection);

            // Sayfa 2: Yaş Grubu (Poliçe + Net Prim sıralaması)
            col.Item().PageBreak();
            col.Item().Element(ComposeAgeGroupSection);

            // Sayfa 3: Sigortalı İli (Poliçe + Net Prim sıralaması)
            col.Item().PageBreak();
            col.Item().Element(ComposeInsuredCitySection);
        });
    }

    private void ComposeKpiSection(IContainer container)
    {
        container.Column(col =>
        {
            col.Item().Element(c => SectionLabel(c, "ÖZET GÖSTERGELER"));

            col.Item().Row(row =>
            {
                row.Spacing(10);

                // 1. Gerçek Kişi
                row.RelativeItem().Element(c => DemoKpiCard(c,
                    "GERÇEK KİŞİ", "#10B981",
                    $"%{_kpi.IndividualRatio:F1}",
                    _kpi.IndividualWoW,
                    "geçen haftaya göre",
                    GetLastWeekRange(),
                    $"%{_kpi.PrevIndividualRatio:F1} — {_kpi.PrevIndividualCount:N0} poliçe",
                    GetPrevWeekRange()));

                // 2. Tüzel Kişi
                row.RelativeItem().Element(c => DemoKpiCard(c,
                    "TÜZEL KİŞİ", "#3B82F6",
                    $"%{_kpi.CorporateRatio:F1}",
                    _kpi.CorporateWoW,
                    "geçen haftaya göre",
                    GetLastWeekRange(),
                    $"%{_kpi.PrevCorporateRatio:F1} — {_kpi.PrevCorporateCount:N0} poliçe",
                    GetPrevWeekRange()));

                // 3. Top Sigortalı İli
                row.RelativeItem().Element(c => DemoKpiCard(c,
                    "TOP SİGORTALI İLİ", "#F59E0B",
                    _kpi.TopPlateCity,
                    _kpi.TopPlateCityWoW,
                    "puan değişim",
                    GetLastWeekRange(),
                    $"{_kpi.PrevTopPlateCity} — %{_kpi.PrevTopPlateCityRatio:F1}",
                    GetPrevWeekRange()));

                // 4. Dominant Yaş Grubu
                row.RelativeItem().Element(c => DemoKpiCard(c,
                    "DOMINANT YAŞ GRUBU", "#8B5CF6",
                    _kpi.DominantAgeGroup,
                    _kpi.DominantAgeWoW,
                    "puan değişim",
                    GetLastWeekRange(),
                    $"{_kpi.PrevDominantAgeGroup} — %{_kpi.PrevDominantAgeRatio:F1}",
                    GetPrevWeekRange()));
            });
        });
    }

    private void ComposeInsuredTypeSection(IContainer container)
    {
        container.Column(col =>
        {
            col.Item().Element(c => SectionLabel(c, "SİGORTALI TÜRÜ DAĞILIMI"));
            col.Item().Border(0.5f).BorderColor("#e2e8f0").Background("#ffffff").Padding(16)
                .Element(c => ComposeDonutWithTable(c, _insuredType, "SİGORTALI TÜRÜ"));
        });
    }

    private void ComposeGenderSection(IContainer container)
    {
        container.Column(col =>
        {
            col.Item().Element(c => SectionLabel(c, "CİNSİYET DAĞILIMI"));
            col.Item().Border(0.5f).BorderColor("#e2e8f0").Background("#ffffff").Padding(16)
                .Element(c => ComposeDonutWithTable(c, _gender, "CİNSİYET"));
        });
    }

    private void ComposeAgeGroupSection(IContainer container)
    {
        container.Column(col =>
        {
            col.Item().Element(c => SectionLabel(c, "YAŞ GRUBU DAĞILIMI"));

            col.Item().Row(row =>
            {
                row.Spacing(16);

                // Sol: Poliçe Sayısına Göre
                row.RelativeItem().Column(c =>
                {
                    c.Item().Text("Poliçe Sayısına Göre").FontSize(9).Bold().FontColor("#1e293b");
                    c.Item().PaddingTop(8).Element(cont => ComposeRankedTable(cont, 
                        _ageGroup.OrderByDescending(x => x.PolicyCount).ToList(), 
                        "YAŞ GRUBU", true));
                });

                // Sağ: Net Prime Göre
                row.RelativeItem().Column(c =>
                {
                    c.Item().Text("Net Prime Göre").FontSize(9).Bold().FontColor("#1e293b");
                    c.Item().PaddingTop(8).Element(cont => ComposeRankedTable(cont, 
                        _ageGroup.OrderByDescending(x => x.NetPremium).ToList(), 
                        "YAŞ GRUBU", false));
                });
            });
        });
    }

    private void ComposeInsuredCitySection(IContainer container)
    {
        container.Column(col =>
        {
            col.Item().Element(c => SectionLabel(c, "SİGORTALI İLİ DAĞILIMI — TOP 10"));

            col.Item().Row(row =>
            {
                row.Spacing(16);

                // Sol: Poliçe Sayısına Göre
                row.RelativeItem().Column(c =>
                {
                    c.Item().Text("Poliçe Sayısına Göre").FontSize(9).Bold().FontColor("#1e293b");
                    c.Item().PaddingTop(8).Element(cont => ComposeRankedTable(cont, 
                        _insuredCity.OrderByDescending(x => x.PolicyCount).Take(10).ToList(), 
                        "İL", true));
                });

                // Sağ: Net Prime Göre
                row.RelativeItem().Column(c =>
                {
                    c.Item().Text("Net Prime Göre").FontSize(9).Bold().FontColor("#1e293b");
                    c.Item().PaddingTop(8).Element(cont => ComposeRankedTable(cont, 
                        _insuredCity.OrderByDescending(x => x.NetPremium).Take(10).ToList(), 
                        "İL", false));
                });
            });
        });
    }

    private void ComposeRankedTable(IContainer container, List<GetDemoDistributionResponse> data, string labelHeader, bool highlightPolicy)
    {
        container.Table(table =>
        {
            table.ColumnsDefinition(cols =>
            {
                cols.ConstantColumn(24);  // #
                cols.RelativeColumn(2f);  // Label
                cols.RelativeColumn();    // Poliçe
                cols.RelativeColumn();    // Net Prim
                cols.RelativeColumn();    // Pay
                cols.RelativeColumn();    // WoW
            });

            // Header
            table.Header(header =>
            {
                header.Cell().Background("#0f3460").Padding(5).Text("#").FontSize(7).Bold().FontColor("#ffffff");
                header.Cell().Background("#0f3460").Padding(5).Text(labelHeader).FontSize(7).Bold().FontColor("#ffffff");
                header.Cell().Background("#0f3460").Padding(5).AlignRight().Text("POLİÇE").FontSize(7).Bold().FontColor("#ffffff");
                header.Cell().Background("#0f3460").Padding(5).AlignRight().Text("NET PRİM").FontSize(7).Bold().FontColor("#ffffff");
                header.Cell().Background("#0f3460").Padding(5).AlignRight().Text("PAY").FontSize(7).Bold().FontColor("#ffffff");
                header.Cell().Background("#0f3460").Padding(5).AlignCenter().Text("WoW").FontSize(7).Bold().FontColor("#ffffff");
            });

            var rank = 1;
            foreach (var item in data)
            {
                var isFirst = rank == 1;
                var bg = isFirst ? "#fef3c7" : (rank % 2 == 0 ? "#f8fafc" : "#ffffff");
                var textColor = isFirst ? "#92400e" : "#1e293b";
                var wowColor = item.WoW >= 0 ? "#10B981" : "#EF4444";

                table.Cell().Background(bg).Padding(5).Text(rank.ToString()).FontSize(8).FontColor(isFirst ? "#f59e0b" : "#94a3b8").Bold();
                table.Cell().Background(bg).Padding(5).Text(item.Label).FontSize(8).Bold().FontColor(textColor);
                table.Cell().Background(bg).Padding(5).AlignRight().Text($"{item.PolicyCount:N0}").FontSize(8).FontColor(highlightPolicy && isFirst ? "#f59e0b" : "#1e293b").Bold();
                table.Cell().Background(bg).Padding(5).AlignRight().Text(FmtPrm(item.NetPremium)).FontSize(8).FontColor(!highlightPolicy && isFirst ? "#f59e0b" : "#1e293b").Bold();
                table.Cell().Background(bg).Padding(5).AlignRight().Text($"%{item.Ratio:F1}").FontSize(8).FontColor("#64748b");
                table.Cell().Background(bg).Padding(5).AlignCenter().Text(FmtWoW(item.WoW)).FontSize(8).Bold().FontColor(wowColor);

                rank++;
            }
        });
    }

    private void ComposeDonutWithTable(IContainer container, List<GetDemoDistributionResponse> data, string labelHeader)
    {
        if (!data.Any()) return;

        container.Row(row =>
        {
            row.Spacing(24);

            // Donut Chart (SVG)
            row.ConstantItem(100).Height(100).Svg(size => DrawDonutChart(data, (int)size.Width, (int)size.Height));

            // Tablo - Tam genişlik
            row.RelativeItem().Table(table =>
            {
                table.ColumnsDefinition(cols =>
                {
                    cols.RelativeColumn(2.5f);  // Label
                    cols.RelativeColumn();       // Poliçe
                    cols.RelativeColumn(1.2f);   // Net Prim
                    cols.RelativeColumn();       // Pay
                    cols.RelativeColumn();       // WoW
                });

                // Header
                table.Header(header =>
                {
                    header.Cell().Background("#f8fafc").Padding(5).Text(labelHeader).FontSize(7).Bold().FontColor("#64748b");
                    header.Cell().Background("#f8fafc").Padding(5).AlignRight().Text("POLİÇE").FontSize(7).Bold().FontColor("#64748b");
                    header.Cell().Background("#f8fafc").Padding(5).AlignRight().Text("NET PRİM").FontSize(7).Bold().FontColor("#64748b");
                    header.Cell().Background("#f8fafc").Padding(5).AlignRight().Text("PAY").FontSize(7).Bold().FontColor("#64748b");
                    header.Cell().Background("#f8fafc").Padding(5).AlignRight().Text("WoW").FontSize(7).Bold().FontColor("#64748b");
                });

                foreach (var (item, idx) in data.Select((x, i) => (x, i)))
                {
                    var color = ChartColors[idx % ChartColors.Count];
                    var wowColor = item.WoW >= 0 ? "#10B981" : "#EF4444";
                    var bg = idx % 2 == 0 ? "#ffffff" : "#f8fafc";

                    table.Cell().Background(bg).Padding(5).Row(r =>
                    {
                        r.ConstantItem(10).Height(10).Background(color);
                        r.ConstantItem(6);
                        r.RelativeItem().AlignMiddle().Text(item.Label).FontSize(8).FontColor("#1e293b");
                    });
                    table.Cell().Background(bg).Padding(5).AlignRight().AlignMiddle().Text($"{item.PolicyCount:N0}").FontSize(9).Bold().FontColor("#1e293b");
                    table.Cell().Background(bg).Padding(5).AlignRight().AlignMiddle().Text(FmtPrm(item.NetPremium)).FontSize(8).FontColor("#475569");
                    table.Cell().Background(bg).Padding(5).AlignRight().AlignMiddle().Text($"%{item.Ratio:F1}").FontSize(8).FontColor("#64748b");
                    table.Cell().Background(bg).Padding(5).AlignRight().AlignMiddle().Text(FmtWoW(item.WoW)).FontSize(8).Bold().FontColor(wowColor);
                }
            });
        });
    }

    private static string DrawDonutChart(List<GetDemoDistributionResponse> data, int width, int height)
    {
        var total = data.Sum(x => x.PolicyCount);
        if (total == 0) return $"<svg width='{width}' height='{height}' xmlns='http://www.w3.org/2000/svg'></svg>";

        var cx = width / 2;
        var cy = height / 2;
        var outerR = Math.Min(width, height) / 2 - 2;
        var innerR = outerR * 0.6;

        var sb = new System.Text.StringBuilder();
        sb.Append($"<svg width='{width}' height='{height}' xmlns='http://www.w3.org/2000/svg'>");

        var startAngle = -90.0;
        foreach (var (item, idx) in data.Select((x, i) => (x, i)))
        {
            var ratio = (double)item.PolicyCount / total;
            var sweepAngle = ratio * 360;
            var color = ChartColors[idx % ChartColors.Count];

            var path = DescribeArc(cx, cy, outerR, innerR, startAngle, startAngle + sweepAngle);
            sb.Append($"<path d='{path}' fill='{color}'/>");

            startAngle += sweepAngle;
        }

        sb.Append("</svg>");
        return sb.ToString();
    }

    private static string DescribeArc(double cx, double cy, double outerR, double innerR, double startAngle, double endAngle)
    {
        var startOuter = PolarToCartesian(cx, cy, outerR, endAngle);
        var endOuter = PolarToCartesian(cx, cy, outerR, startAngle);
        var startInner = PolarToCartesian(cx, cy, innerR, endAngle);
        var endInner = PolarToCartesian(cx, cy, innerR, startAngle);

        var largeArc = endAngle - startAngle <= 180 ? "0" : "1";

        return $"M {startOuter.x:F2} {startOuter.y:F2} " +
               $"A {outerR:F2} {outerR:F2} 0 {largeArc} 0 {endOuter.x:F2} {endOuter.y:F2} " +
               $"L {endInner.x:F2} {endInner.y:F2} " +
               $"A {innerR:F2} {innerR:F2} 0 {largeArc} 1 {startInner.x:F2} {startInner.y:F2} Z";
    }

    private static (double x, double y) PolarToCartesian(double cx, double cy, double r, double angleDeg)
    {
        var angleRad = angleDeg * Math.PI / 180.0;
        return (cx + r * Math.Cos(angleRad), cy + r * Math.Sin(angleRad));
    }

    private static void DemoKpiCard(IContainer container,
        string label, string accentColor,
        string value, decimal wow, string wowLabel,
        string weekRange, string prevValue, string prevRange)
    {
        var wowColor = wow >= 0 ? "#10B981" : "#EF4444";
        var wowSign = wow > 0 ? "+" : "";

        container
            .Border(0.5f).BorderColor("#e2e8f0")
            .BorderTop(2f).BorderColor(accentColor)
            .Padding(11)
            .Column(col =>
            {
                // Label
                col.Item().Text(label).FontSize(7).Bold().FontColor("#64748b").LetterSpacing(0.05f);
                
                // Week Range
                col.Item().PaddingTop(3).Text(weekRange).FontSize(7).FontColor("#94a3b8");
                
                // Value (büyük)
                col.Item().PaddingTop(5).Text(value).FontSize(20).Bold().FontColor("#0f172a");
                
                // WoW Badge
                col.Item().PaddingTop(4).Row(r =>
                {
                    r.AutoItem()
                        .Background(wow >= 0 ? "#d1fae5" : "#fee2e2")
                        .Padding(3).PaddingHorizontal(6)
                        .Text($"{wowSign}{wow:F1} {wowLabel}").FontSize(7).Bold().FontColor(wowColor);
                });

                // Önceki Hafta - Border ile ayır
                col.Item().PaddingTop(8).BorderTop(0.5f).BorderColor("#f1f5f9").PaddingTop(6).Column(prev =>
                {
                    prev.Item().Text(prevRange).FontSize(7).FontColor("#94a3b8");
                    prev.Item().PaddingTop(2).Text(prevValue).FontSize(8).Bold().FontColor("#475569");
                });
            });
    }
}
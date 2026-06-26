using ClosedXML.Excel;
using DataAnalysis.Application.Features.Company.Queries.GetCompanyHeatmap;

namespace DataAnalysis.Infrastructure.Excel;

public class CompanyHeatmapExcelExport
{
    public byte[] Build(List<GetCompanyHeatmapResponse> data, string productGroup, string filterSummary = "")
    {
        using var workbook = new XLWorkbook();

        var companies = data.Select(d => d.Company).Distinct().ToList();
        var weeks     = data.Select(d => d.Week).Distinct().ToList();
        var dateRange = $"{weeks.First()} - {weeks.Last()}";
        var pg = string.IsNullOrWhiteSpace(filterSummary) ? productGroup : $"{productGroup} ({filterSummary})";

        BuildPremiumSheet(workbook, data, companies, weeks, pg, dateRange);
        BuildPolicyRatioSheet(workbook, data, companies, weeks, pg, dateRange);

        return ExcelBuilder.ToBytes(workbook);
    }

    private void BuildPremiumSheet(XLWorkbook workbook, List<GetCompanyHeatmapResponse> data,
        List<string> companies, List<string> weeks, string productGroup, string dateRange)
    {
        var ws = workbook.AddWorksheet("Ort. Yazılan Prim");
        var valueMap = data.ToDictionary(d => $"{d.Company}__{d.Week}", d => d.AvgNetPremium);

        ExcelBuilder.WriteTitle(ws, $"Şirket Geçiş — Ort. Yazılan Prim — {productGroup} — {dateRange}", weeks.Count + 1);

        ExcelBuilder.WriteColumnHeader(ws.Cell(3, 1), "Önceki Şirket");
        for (int w = 0; w < weeks.Count; w++)
            ExcelBuilder.WriteColumnHeader(ws.Cell(3, w + 2), weeks[w]);

        for (int c = 0; c < companies.Count; c++)
        {
            int row     = 4 + c;
            var company = companies[c];

            ws.Cell(row, 1).Value               = company;
            ws.Cell(row, 1).Style.Font.Bold     = true;
            ws.Cell(row, 1).Style.Font.FontSize = 10;

            var rowValues = weeks
                .Select(w => valueMap.TryGetValue($"{company}__{w}", out var v) && v > 0 ? v : (decimal?)null)
                .Where(v => v.HasValue).Select(v => v!.Value).ToList();

            var rowMin = rowValues.Count > 0 ? rowValues.Min() : 0;
            var rowMax = rowValues.Count > 0 ? rowValues.Max() : 0;

            for (int w = 0; w < weeks.Count; w++)
            {
                var cell = ws.Cell(row, w + 2);
                var key  = $"{company}__{weeks[w]}";

                if (valueMap.TryGetValue(key, out var val) && val > 0)
                {
                    cell.Value                      = (double)val;
                    cell.Style.NumberFormat.Format  = "#,##0";
                    cell.Style.Font.FontSize        = 10;
                    cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    cell.Style.Fill.BackgroundColor = ExcelBuilder.GetHeatColor(val, rowMin, rowMax);
                    cell.Style.Font.FontColor       = XLColor.FromArgb(26, 26, 26);
                }
                else
                {
                    cell.Value                      = "—";
                    cell.Style.Font.FontColor       = XLColor.FromArgb(148, 163, 184);
                    cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                }
            }
        }

        ws.SheetView.Freeze(3, 1);
        ws.Column(1).AdjustToContents();
        for (int w = 2; w <= weeks.Count + 1; w++)
            ws.Column(w).AdjustToContents();
        ws.ShowGridLines = false;
    }

    private void BuildPolicyRatioSheet(XLWorkbook workbook, List<GetCompanyHeatmapResponse> data,
        List<string> companies, List<string> weeks, string productGroup, string dateRange)
    {
        var ws = workbook.AddWorksheet("Poliçe Payı");
        var valueMap = data.ToDictionary(d => $"{d.Company}__{d.Week}", d => d.PolicyRatio);

        ExcelBuilder.WriteTitle(ws, $"Şirket Geçiş — Poliçe Payı (%) — {productGroup} — {dateRange}", weeks.Count + 1);

        ExcelBuilder.WriteColumnHeader(ws.Cell(3, 1), "Önceki Şirket");
        for (int w = 0; w < weeks.Count; w++)
            ExcelBuilder.WriteColumnHeader(ws.Cell(3, w + 2), weeks[w]);

        for (int c = 0; c < companies.Count; c++)
        {
            int row     = 4 + c;
            var company = companies[c];

            ws.Cell(row, 1).Value               = company;
            ws.Cell(row, 1).Style.Font.Bold     = true;
            ws.Cell(row, 1).Style.Font.FontSize = 10;

            var rowValues = weeks
                .Select(w => valueMap.TryGetValue($"{company}__{w}", out var v) && v > 0 ? v : (decimal?)null)
                .Where(v => v.HasValue).Select(v => v!.Value).ToList();

            var rowMin = rowValues.Count > 0 ? rowValues.Min() : 0;
            var rowMax = rowValues.Count > 0 ? rowValues.Max() : 0;

            for (int w = 0; w < weeks.Count; w++)
            {
                var cell = ws.Cell(row, w + 2);
                var key  = $"{company}__{weeks[w]}";

                if (valueMap.TryGetValue(key, out var val) && val > 0)
                {
                    cell.Value                      = (double)val / 100;
                    cell.Style.NumberFormat.Format  = "0.0%";
                    cell.Style.Font.FontSize        = 10;
                    cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    cell.Style.Fill.BackgroundColor = ExcelBuilder.GetHeatColor(val, rowMin, rowMax);
                    cell.Style.Font.FontColor       = XLColor.FromArgb(26, 26, 26);
                }
                else
                {
                    cell.Value                      = "—";
                    cell.Style.Font.FontColor       = XLColor.FromArgb(148, 163, 184);
                    cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                }
            }
        }

        ws.SheetView.Freeze(3, 1);
        ws.Column(1).AdjustToContents();
        for (int w = 2; w <= weeks.Count + 1; w++)
            ws.Column(w).AdjustToContents();
        ws.ShowGridLines = false;
    }
}
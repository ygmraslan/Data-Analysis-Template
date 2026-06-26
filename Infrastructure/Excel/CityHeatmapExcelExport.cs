using ClosedXML.Excel;
using DataAnalysis.Application.Features.City.Queries.GetCityHeatmap;

namespace DataAnalysis.Infrastructure.Excel;

public class CityHeatmapExcelExport
{
    public byte[] Build(List<CityHeatmapResponse> data, string productGroup, string filterSummary = "")
    {
        using var workbook = new XLWorkbook();

        var cities    = data.Select(d => d.City).Distinct().ToList();
        var weeks     = data.Select(d => d.Week).Distinct().ToList();
        var dateRange = $"{weeks.First()} - {weeks.Last()}";
        var pg = string.IsNullOrWhiteSpace(filterSummary) ? productGroup : $"{productGroup} ({filterSummary})";

        BuildPremiumSheet(workbook, data, cities, weeks, pg, dateRange);
        BuildPolicyRatioSheet(workbook, data, cities, weeks, pg, dateRange);

        return ExcelBuilder.ToBytes(workbook);
    }

    private void BuildPremiumSheet(XLWorkbook workbook, List<CityHeatmapResponse> data,
        List<string> cities, List<string> weeks, string productGroup, string dateRange)
    {
        var ws = workbook.AddWorksheet("Ort. Yazılan Prim");
        var valueMap = data.ToDictionary(d => $"{d.City}__{d.Week}", d => d.AvgNetPremium);

        ExcelBuilder.WriteTitle(ws, $"İl — Ort. Yazılan Prim — {productGroup} — {dateRange}", weeks.Count + 1);

        ExcelBuilder.WriteColumnHeader(ws.Cell(3, 1), "İl");
        for (int w = 0; w < weeks.Count; w++)
            ExcelBuilder.WriteColumnHeader(ws.Cell(3, w + 2), weeks[w]);

        for (int c = 0; c < cities.Count; c++)
        {
            int row  = 4 + c;
            var city = cities[c];

            ws.Cell(row, 1).Value               = city;
            ws.Cell(row, 1).Style.Font.Bold     = true;
            ws.Cell(row, 1).Style.Font.FontSize = 10;

            var rowValues = weeks
                .Select(w => valueMap.TryGetValue($"{city}__{w}", out var v) && v > 0 ? v : (decimal?)null)
                .Where(v => v.HasValue).Select(v => v!.Value).ToList();

            var rowMin = rowValues.Count > 0 ? rowValues.Min() : 0;
            var rowMax = rowValues.Count > 0 ? rowValues.Max() : 0;

            for (int w = 0; w < weeks.Count; w++)
            {
                var cell = ws.Cell(row, w + 2);
                var key  = $"{city}__{weeks[w]}";

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

    private void BuildPolicyRatioSheet(XLWorkbook workbook, List<CityHeatmapResponse> data,
        List<string> cities, List<string> weeks, string productGroup, string dateRange)
    {
        var ws = workbook.AddWorksheet("Poliçe Payı");
        var valueMap = data.ToDictionary(d => $"{d.City}__{d.Week}", d => d.PolicyRatio);

        ExcelBuilder.WriteTitle(ws, $"İl — Poliçe Payı (%) — {productGroup} — {dateRange}", weeks.Count + 1);

        ExcelBuilder.WriteColumnHeader(ws.Cell(3, 1), "İl");
        for (int w = 0; w < weeks.Count; w++)
            ExcelBuilder.WriteColumnHeader(ws.Cell(3, w + 2), weeks[w]);

        for (int c = 0; c < cities.Count; c++)
        {
            int row  = 4 + c;
            var city = cities[c];

            ws.Cell(row, 1).Value               = city;
            ws.Cell(row, 1).Style.Font.Bold     = true;
            ws.Cell(row, 1).Style.Font.FontSize = 10;

            var rowValues = weeks
                .Select(w => valueMap.TryGetValue($"{city}__{w}", out var v) && v > 0 ? v : (decimal?)null)
                .Where(v => v.HasValue).Select(v => v!.Value).ToList();

            var rowMin = rowValues.Count > 0 ? rowValues.Min() : 0;
            var rowMax = rowValues.Count > 0 ? rowValues.Max() : 0;

            for (int w = 0; w < weeks.Count; w++)
            {
                var cell = ws.Cell(row, w + 2);
                var key  = $"{city}__{weeks[w]}";

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
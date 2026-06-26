using ClosedXML.Excel;
using DataAnalysis.Application.Features.Region.Queries.GetRegionHeatmap;

namespace DataAnalysis.Infrastructure.Excel;

public class RegionHeatmapExcelExport
{
    public byte[] Build(List<GetRegionHeatmapQueryResponse> data, string productGroup, string filterSummary = "")
    {
        using var workbook = new XLWorkbook();

        var regions   = data.Select(d => d.Region).Distinct().ToList();
        var weeks     = data.Select(d => d.Week).Distinct().ToList();
        var dateRange = $"{weeks.First()} - {weeks.Last()}";
        var pg = string.IsNullOrWhiteSpace(filterSummary) ? productGroup : $"{productGroup} ({filterSummary})";

        BuildPremiumSheet(workbook, data, regions, weeks, pg, dateRange);
        BuildPolicyRatioSheet(workbook, data, regions, weeks, pg, dateRange);

        return ExcelBuilder.ToBytes(workbook);
    }
    private void BuildPremiumSheet(XLWorkbook workbook, List<GetRegionHeatmapQueryResponse> data,
        List<string> regions, List<string> weeks, string productGroup, string dateRange)
    {
        var ws = workbook.AddWorksheet("Ort. Yazılan Prim");
        var valueMap = data.ToDictionary(d => $"{d.Region}__{d.Week}", d => d.AvgNetPremium);

        ExcelBuilder.WriteTitle(ws, $"Bölge — Ort. Yazılan Prim — {productGroup} — {dateRange}", weeks.Count + 1);

        ExcelBuilder.WriteColumnHeader(ws.Cell(3, 1), "Bölge");
        for (int w = 0; w < weeks.Count; w++)
            ExcelBuilder.WriteColumnHeader(ws.Cell(3, w + 2), weeks[w]);

        for (int r = 0; r < regions.Count; r++)
        {
            int row    = 4 + r;
            var region = regions[r];

            ws.Cell(row, 1).Value = region;
            ws.Cell(row, 1).Style.Font.Bold     = true;
            ws.Cell(row, 1).Style.Font.FontSize = 10;

            var rowValues = weeks
                .Select(w => valueMap.TryGetValue($"{region}__{w}", out var v) && v > 0 ? v : (decimal?)null)
                .Where(v => v.HasValue)
                .Select(v => v!.Value)
                .ToList();

            var rowMin = rowValues.Count > 0 ? rowValues.Min() : 0;
            var rowMax = rowValues.Count > 0 ? rowValues.Max() : 0;

            for (int w = 0; w < weeks.Count; w++)
            {
                var cell = ws.Cell(row, w + 2);
                var key  = $"{region}__{weeks[w]}";

                if (valueMap.TryGetValue(key, out var val) && val > 0)
                {
                    cell.Value = (double)val;
                    cell.Style.NumberFormat.Format    = "#,##0";
                    cell.Style.Font.FontSize          = 10;
                    cell.Style.Alignment.Horizontal   = XLAlignmentHorizontalValues.Center;
                    cell.Style.Fill.BackgroundColor   = ExcelBuilder.GetHeatColor(val, rowMin, rowMax);
                    cell.Style.Font.FontColor         = XLColor.FromArgb(26, 26, 26);
                }
                else
                {
                    cell.Value = "—";
                    cell.Style.Font.FontColor       = XLColor.FromArgb(148, 163, 184);
                    cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                }
            }
        }

        ws.SheetView.FreezeColumns(1);
        ws.Column(1).AdjustToContents();
        for (int w = 2; w <= weeks.Count + 1; w++)
            ws.Column(w).AdjustToContents();
        ws.ShowGridLines = false;
    }

    private void BuildPolicyRatioSheet(XLWorkbook workbook, List<GetRegionHeatmapQueryResponse> data,
        List<string> regions, List<string> weeks, string productGroup, string dateRange)
    {
        var ws = workbook.AddWorksheet("Poliçe Payı");
        var valueMap = data.ToDictionary(d => $"{d.Region}__{d.Week}", d => d.PolicyRatio);

        ExcelBuilder.WriteTitle(ws, $"Bölge — Poliçe Payı (%) — {productGroup} — {dateRange}", weeks.Count + 1);

        ExcelBuilder.WriteColumnHeader(ws.Cell(3, 1), "Bölge");
        for (int w = 0; w < weeks.Count; w++)
            ExcelBuilder.WriteColumnHeader(ws.Cell(3, w + 2), weeks[w]);

        for (int r = 0; r < regions.Count; r++)
        {
            int row    = 4 + r;
            var region = regions[r];

            ws.Cell(row, 1).Value = region;
            ws.Cell(row, 1).Style.Font.Bold     = true;
            ws.Cell(row, 1).Style.Font.FontSize = 10;

            var rowValues = weeks
                .Select(w => valueMap.TryGetValue($"{region}__{w}", out var v) && v > 0 ? v : (decimal?)null)
                .Where(v => v.HasValue)
                .Select(v => v!.Value)
                .ToList();

            var rowMin = rowValues.Count > 0 ? rowValues.Min() : 0;
            var rowMax = rowValues.Count > 0 ? rowValues.Max() : 0;

            for (int w = 0; w < weeks.Count; w++)
            {
                var cell = ws.Cell(row, w + 2);
                var key  = $"{region}__{weeks[w]}";

                if (valueMap.TryGetValue(key, out var val) && val > 0)
                {
                    cell.Value = (double)val / 100;
                    cell.Style.NumberFormat.Format    = "0.0%";
                    cell.Style.Font.FontSize          = 10;
                    cell.Style.Alignment.Horizontal   = XLAlignmentHorizontalValues.Center;
                    cell.Style.Fill.BackgroundColor   = ExcelBuilder.GetHeatColor(val, rowMin, rowMax);
                    cell.Style.Font.FontColor         = XLColor.FromArgb(26, 26, 26);
                }
                else
                {
                    cell.Value = "—";
                    cell.Style.Font.FontColor       = XLColor.FromArgb(148, 163, 184);
                    cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                }
            }
        }

        ws.SheetView.FreezeColumns(1);
        ws.Column(1).AdjustToContents();
        for (int w = 2; w <= weeks.Count + 1; w++)
            ws.Column(w).AdjustToContents();
        ws.ShowGridLines = false;
    }
}
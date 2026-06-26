using ClosedXML.Excel;
using DataAnalysis.Application.Features.Agency.Queries.GetAgencyHeatmap;

namespace DataAnalysis.Infrastructure.Excel;

public class AgencyHeatmapExcelExport
{
    public byte[] Build(List<GetAgencyHeatmapResponse> dataList, string productGroup, string filterSummary = "")
    {
        var data = dataList.FirstOrDefault() ?? new GetAgencyHeatmapResponse();

        using var workbook = new XLWorkbook();

        var weeks = data.Weeks;
        var dateRange = weeks.Count > 0 ? $"{weeks.First()} - {weeks.Last()}" : "";
        var pg = string.IsNullOrWhiteSpace(filterSummary) ? productGroup : $"{productGroup} ({filterSummary})";

        BuildPremiumSheet(workbook, data, weeks, pg, dateRange);
        BuildPolicyRatioSheet(workbook, data, weeks, pg, dateRange);

        return ExcelBuilder.ToBytes(workbook);
    }

    private void BuildPremiumSheet(XLWorkbook workbook, GetAgencyHeatmapResponse data,
        List<string> weeks, string productGroup, string dateRange)
    {
        var ws = workbook.AddWorksheet("Ort. Yazılan Prim");
        
        ExcelBuilder.WriteTitle(ws, $"Acente — Ort. Yazılan Prim — {productGroup} — {dateRange}", weeks.Count + 2);

        // Header row
        ExcelBuilder.WriteColumnHeader(ws.Cell(3, 1), "Acente Kodu");
        ExcelBuilder.WriteColumnHeader(ws.Cell(3, 2), "Acente Adı");
        for (int w = 0; w < weeks.Count; w++)
            ExcelBuilder.WriteColumnHeader(ws.Cell(3, w + 3), weeks[w]);

        // Data rows
        for (int r = 0; r < data.Rows.Count; r++)
        {
            int row = 4 + r;
            var agencyRow = data.Rows[r];

            ws.Cell(row, 1).Value = agencyRow.AgencyCode;
            ws.Cell(row, 1).Style.Font.FontSize = 10;
            
            ws.Cell(row, 2).Value = agencyRow.AgencyName;
            ws.Cell(row, 2).Style.Font.Bold = true;
            ws.Cell(row, 2).Style.Font.FontSize = 10;

            var rowValues = agencyRow.Cells
                .Where(c => c.AvgPremium > 0)
                .Select(c => c.AvgPremium)
                .ToList();

            var rowMin = rowValues.Count > 0 ? rowValues.Min() : 0;
            var rowMax = rowValues.Count > 0 ? rowValues.Max() : 0;

            for (int w = 0; w < weeks.Count; w++)
            {
                var cell = ws.Cell(row, w + 3);
                var heatCell = agencyRow.Cells.FirstOrDefault(c => c.Week == weeks[w]);

                if (heatCell != null && heatCell.AvgPremium > 0)
                {
                    cell.Value = (double)heatCell.AvgPremium;
                    cell.Style.NumberFormat.Format = "#,##0";
                    cell.Style.Font.FontSize = 10;
                    cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    cell.Style.Fill.BackgroundColor = ExcelBuilder.GetHeatColor(heatCell.AvgPremium, rowMin, rowMax);
                    cell.Style.Font.FontColor = XLColor.FromArgb(26, 26, 26);
                }
                else
                {
                    cell.Value = "—";
                    cell.Style.Font.FontColor = XLColor.FromArgb(148, 163, 184);
                    cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                }
            }
        }

        ws.SheetView.Freeze(3, 2);
        ws.Column(1).AdjustToContents();
        ws.Column(2).AdjustToContents();
        for (int w = 3; w <= weeks.Count + 2; w++)
            ws.Column(w).AdjustToContents();
        ws.ShowGridLines = false;
    }

    private void BuildPolicyRatioSheet(XLWorkbook workbook, GetAgencyHeatmapResponse data,
        List<string> weeks, string productGroup, string dateRange)
    {
        var ws = workbook.AddWorksheet("Poliçe Payı");
        
        ExcelBuilder.WriteTitle(ws, $"Acente — Poliçe Payı (%) — {productGroup} — {dateRange}", weeks.Count + 2);

        // Header row
        ExcelBuilder.WriteColumnHeader(ws.Cell(3, 1), "Acente Kodu");
        ExcelBuilder.WriteColumnHeader(ws.Cell(3, 2), "Acente Adı");
        for (int w = 0; w < weeks.Count; w++)
            ExcelBuilder.WriteColumnHeader(ws.Cell(3, w + 3), weeks[w]);

        // Data rows
        for (int r = 0; r < data.Rows.Count; r++)
        {
            int row = 4 + r;
            var agencyRow = data.Rows[r];

            ws.Cell(row, 1).Value = agencyRow.AgencyCode;
            ws.Cell(row, 1).Style.Font.FontSize = 10;
            
            ws.Cell(row, 2).Value = agencyRow.AgencyName;
            ws.Cell(row, 2).Style.Font.Bold = true;
            ws.Cell(row, 2).Style.Font.FontSize = 10;

            var rowValues = agencyRow.Cells
                .Where(c => c.PolicyRatio > 0)
                .Select(c => c.PolicyRatio)
                .ToList();

            var rowMin = rowValues.Count > 0 ? rowValues.Min() : 0;
            var rowMax = rowValues.Count > 0 ? rowValues.Max() : 0;

            for (int w = 0; w < weeks.Count; w++)
            {
                var cell = ws.Cell(row, w + 3);
                var heatCell = agencyRow.Cells.FirstOrDefault(c => c.Week == weeks[w]);

                if (heatCell != null && heatCell.PolicyRatio > 0)
                {
                    cell.Value = (double)heatCell.PolicyRatio / 100;
                    cell.Style.NumberFormat.Format = "0.0%";
                    cell.Style.Font.FontSize = 10;
                    cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    cell.Style.Fill.BackgroundColor = ExcelBuilder.GetHeatColor(heatCell.PolicyRatio, rowMin, rowMax);
                    cell.Style.Font.FontColor = XLColor.FromArgb(26, 26, 26);
                }
                else
                {
                    cell.Value = "—";
                    cell.Style.Font.FontColor = XLColor.FromArgb(148, 163, 184);
                    cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                }
            }
        }

        ws.SheetView.Freeze(3, 2);
        ws.Column(1).AdjustToContents();
        ws.Column(2).AdjustToContents();
        for (int w = 3; w <= weeks.Count + 2; w++)
            ws.Column(w).AdjustToContents();
        ws.ShowGridLines = false;
    }
}
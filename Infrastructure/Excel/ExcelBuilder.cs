using ClosedXML.Excel;

namespace DataAnalysis.Infrastructure.Excel;

public static class ExcelBuilder
{
    private static readonly (int R, int G, int B) HeatLow  = (248, 105, 107);
    private static readonly (int R, int G, int B) HeatMid  = (255, 235, 132);
    private static readonly (int R, int G, int B) HeatHigh = (99,  190, 123);

    public static byte[] ToBytes(XLWorkbook workbook)
    {
        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    public static void WriteTitle(IXLWorksheet ws, string text, int colCount, int row = 1)
    {
        var cell = ws.Cell(row, 1);
        cell.Value = text;
        cell.Style.Font.Bold = true;
        cell.Style.Font.FontSize = 12;
        cell.Style.Font.FontColor = XLColor.FromArgb(255, 255, 255);
        cell.Style.Fill.BackgroundColor = XLColor.FromArgb(15, 31, 60);
        cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        ws.Row(row).Height = 22;
        ws.Range(row, 1, row, colCount).Merge();
    }

    public static void WriteColumnHeader(IXLCell cell, string text)
    {
        cell.Value = text;
        cell.Style.Font.Bold = true;
        cell.Style.Font.FontSize = 10;
        cell.Style.Font.FontColor = XLColor.FromArgb(255, 255, 255);
        cell.Style.Fill.BackgroundColor = XLColor.FromArgb(15, 31, 60);
        cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
    }

    public static XLColor GetHeatColor(decimal value, decimal min, decimal max)
    {
        if (max == min) return XLColor.FromArgb(HeatMid.R, HeatMid.G, HeatMid.B);

        var ratio = (double)(value - min) / (double)(max - min);

        int r, g, b;
        if (ratio <= 0.5)
        {
            var t = ratio * 2;
            r = Lerp(HeatLow.R, HeatMid.R, t);
            g = Lerp(HeatLow.G, HeatMid.G, t);
            b = Lerp(HeatLow.B, HeatMid.B, t);
        }
        else
        {
            var t = (ratio - 0.5) * 2;
            r = Lerp(HeatMid.R, HeatHigh.R, t);
            g = Lerp(HeatMid.G, HeatHigh.G, t);
            b = Lerp(HeatMid.B, HeatHigh.B, t);
        }

        return XLColor.FromArgb(r, g, b);
    }

    private static int Lerp(int from, int to, double t) =>
        (int)Math.Round(from + (to - from) * t);
}
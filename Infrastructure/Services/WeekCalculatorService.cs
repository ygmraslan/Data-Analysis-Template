using System.Globalization;
using DataAnalysis.Application.Common.Interfaces;
using DataAnalysis.Application.Features.ExecSummary.Dtos;

namespace DataAnalysis.Infrastructure.Services;

public class WeekCalculatorService : IWeekCalculatorService
{
    private static readonly CultureInfo TurkishCulture = new("tr-TR");
    
    private static readonly string[] MonthNames = 
    {
        "", "Ocak", "Şubat", "Mart", "Nisan", "Mayıs", "Haziran",
        "Temmuz", "Ağustos", "Eylül", "Ekim", "Kasım", "Aralık"
    };

    public List<WeekRangeDto> GetAvailableWeeks(int year, int month)
    {
        var weeks = new List<WeekRangeDto>();
        
        var firstDayOfMonth = new DateTime(year, month, 1);
        var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
        
        var firstMonday = firstDayOfMonth;
        while (firstMonday.DayOfWeek != DayOfWeek.Monday)
        {
            firstMonday = firstMonday.AddDays(1);
        }
        
        if (firstDayOfMonth.DayOfWeek != DayOfWeek.Monday)
        {
            var prevMonday = firstDayOfMonth;
            while (prevMonday.DayOfWeek != DayOfWeek.Monday)
            {
                prevMonday = prevMonday.AddDays(-1);
            }
            var prevSunday = prevMonday.AddDays(6);
            
            if (prevSunday >= firstDayOfMonth)
            {
                weeks.Add(CreateWeekRange(prevMonday, prevSunday, weeks.Count));
            }
        }
        
        var currentMonday = firstMonday;
        while (currentMonday <= lastDayOfMonth)
        {
            var sunday = currentMonday.AddDays(6);
            weeks.Add(CreateWeekRange(currentMonday, sunday, weeks.Count));
            currentMonday = currentMonday.AddDays(7);
        }
        
        return weeks;
    }

    public List<WeekRangeDto> GetLast10Weeks(DateTime weekEnd)
    {
        var weeks = new List<WeekRangeDto>();

        var sunday = weekEnd;
        while (sunday.DayOfWeek != DayOfWeek.Sunday)
        {
            sunday = sunday.AddDays(1);
        }

        for (int i = 9; i >= 0; i--)
        {
            var currentSunday = sunday.AddDays(-7 * i);
            var currentMonday = currentSunday.AddDays(-6);
            weeks.Add(CreateWeekRange(currentMonday, currentSunday, 9 - i));
        }
        
        return weeks;
    }

    public WeekRangeDto GetCurrentWeek()
    {
        // En son tamamlanmış haftayı döndür (bugün Pazar değilse geçen haftayı)
        var today = DateTime.Today;
        
        // Bugün Pazar ise bu haftayı döndür, değilse geçen haftayı
        if (today.DayOfWeek == DayOfWeek.Sunday)
        {
            return GetWeekByDate(today);
        }
        
        // Geçen Pazar'ı bul
        var lastSunday = today;
        while (lastSunday.DayOfWeek != DayOfWeek.Sunday)
        {
            lastSunday = lastSunday.AddDays(-1);
        }
        
        return GetWeekByDate(lastSunday);
    }

    public WeekRangeDto GetWeekByDate(DateTime date)
    {
        var monday = date;
        while (monday.DayOfWeek != DayOfWeek.Monday)
        {
            monday = monday.AddDays(-1);
        }
        
        var sunday = monday.AddDays(6);
        
        return CreateWeekRange(monday, sunday, 0);
    }

    private WeekRangeDto CreateWeekRange(DateTime monday, DateTime sunday, int weekIndex)
    {
        return new WeekRangeDto
        {
            StartDate = monday.Date,
            EndDate = sunday.Date.AddHours(23).AddMinutes(59).AddSeconds(59),
            Year = monday.Year,
            Month = monday.Month,
            WeekIndex = weekIndex,
            DisplayText = FormatDisplayText(monday, sunday)
        };
    }

    private string FormatDisplayText(DateTime monday, DateTime sunday)
    {
        if (monday.Month == sunday.Month)
        {
            return $"{monday.Day} - {sunday.Day} {MonthNames[monday.Month]} {monday.Year}";
        }

        if (monday.Year == sunday.Year)
        {
            return $"{monday.Day} {MonthNames[monday.Month].Substring(0, 3)} - {sunday.Day} {MonthNames[sunday.Month].Substring(0, 3)} {monday.Year}";
        }

        return $"{monday.Day} {MonthNames[monday.Month].Substring(0, 3)} {monday.Year} - {sunday.Day} {MonthNames[sunday.Month].Substring(0, 3)} {sunday.Year}";
    }
}
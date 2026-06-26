using DataAnalysis.Application.Features.ExecSummary.Dtos;

namespace DataAnalysis.Application.Common.Interfaces;

public interface IWeekCalculatorService
{
    List<WeekRangeDto> GetAvailableWeeks(int year, int month);
    List<WeekRangeDto> GetLast10Weeks(DateTime weekEnd);
    WeekRangeDto GetCurrentWeek();
    WeekRangeDto GetWeekByDate(DateTime date);
}
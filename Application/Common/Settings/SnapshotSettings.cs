namespace DataAnalysis.Application.Common.Settings;

public class SnapshotSettings
{
    public bool Enabled { get; set; } = true;
    public DayOfWeek RunDay { get; set; } = DayOfWeek.Monday;
    public string RunTime { get; set; } = "03:00";
    public int OldVersionGraceMinutes { get; set; } = 5;
    public bool RunOnStartup { get; set; } = false;
}
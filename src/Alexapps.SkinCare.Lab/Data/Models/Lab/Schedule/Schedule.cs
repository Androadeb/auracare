namespace starterkit.Data.Models.Lab.Schedule;

public class Schedule
{
    public string Id { get; set; } = string.Empty;
    public string DayName { get; set; } = string.Empty;
    public TimeSpan OpeningTime { get; set; }
    public TimeSpan ClosingTime { get; set; }
    public bool IsOpen { get; set; }
    public int CapacityPerHour { get; set; }
}


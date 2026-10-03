using System.ComponentModel.DataAnnotations;

namespace starterkit.Data.Models.Lab.Schedule;

public class UpdateScheduleCommand
{
    public string Id { get; set; } = string.Empty;
    [Required] public TimeSpan OpeningTime { get; set; }
    [Required] public TimeSpan ClosingTime { get; set; }

    public bool IsOpen { get; set; }

    [Range(1, 100, ErrorMessage = "Capacity must be between 1 and 100.")]
    public int CapacityPerHour { get; set; }
}


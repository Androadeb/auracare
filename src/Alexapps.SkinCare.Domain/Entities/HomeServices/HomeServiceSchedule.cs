using Alexapps.SkinCare.Entities.Base;
using System;

namespace Alexapps.SkinCare.Entities.HomeServices
{
    public class HomeServiceSchedule : BaseEntity
    {
        public Guid HomeServiceId { get; set; }
        public HomeService HomeService { get; set; }

        public DayOfWeek DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public bool IsAvailable { get; set; } = true;
    }
}

using Alexapps.SkinCare.Entities.Base;
using Alexapps.SkinCare.Entities.HomeServices;
using System;

namespace Alexapps.SkinCare.Entities.Doctors
{
    public class DoctorSchedule : BaseEntity
    {
        public Guid DoctorId { get; set; }
        public Doctor Doctor { get; set; }

        public DayOfWeek DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public bool IsAvailable { get; set; } = true;
    }
}

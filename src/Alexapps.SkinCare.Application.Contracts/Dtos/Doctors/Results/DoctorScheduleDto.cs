using System;
using Volo.Abp.Application.Dtos;

namespace Alexapps.SkinCare.Dtos.Doctors.Results
{
    public class DoctorScheduleDto : EntityDto<Guid>
    {
      
        public DayOfWeek DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public bool IsAvailable { get; set; }
    }
}

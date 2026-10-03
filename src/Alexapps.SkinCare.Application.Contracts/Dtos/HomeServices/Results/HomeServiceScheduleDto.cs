using System;
using Volo.Abp.Application.Dtos;

namespace Alexapps.SkinCare.Dtos.HomeServices.Results
{
    public class HomeServiceScheduleDto : EntityDto<Guid>
    {
        public Guid HomeServiceId { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public bool IsAvailable { get; set; }
    }
}

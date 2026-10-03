using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace Alexapps.SkinCare.Dtos.LabSchedule.Result
{
    public class LabScheduleResult : EntityDto<Guid>
    {

        public DayOfWeek DayOfWeek { get; set; }
        public string DayName => DayOfWeek.ToString(); 
        public TimeSpan OpeningTime { get; set; }
        public TimeSpan ClosingTime { get; set; }
        public bool IsOpen { get; set; }
        public int CapacityPerHour { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alexapps.SkinCare.Dtos.LabSchedule.Command
{
    public class UpdateLabScheduleItemDto
    {
        public Guid Id { get; set; }
        public TimeSpan OpeningTime { get; set; }
        public TimeSpan ClosingTime { get; set; }
        public bool IsOpen { get; set; }
        public int CapacityPerHour { get; set; }
    }

   
    public class UpdateAllLabSchedulesDto
    {
        public List<UpdateLabScheduleItemDto> Schedules { get; set; } = new();
    }
}

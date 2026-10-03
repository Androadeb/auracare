using Alexapps.SkinCare.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alexapps.SkinCare.Entities.LABs
{
    public class LabSchedule : BaseEntity
    {
        public Guid LabId { get; set; }
        public Lab Lab { get; set; }

        public DayOfWeek DayOfWeek { get; set; } // من 0 لـ 6
        public TimeSpan OpeningTime { get; set; } // مثلاً 08:00:00
        public TimeSpan ClosingTime { get; set; } // مثلاً 22:00:00
        public bool IsOpen { get; set; }
        public int CapacityPerHour { get; set; }
    }
}

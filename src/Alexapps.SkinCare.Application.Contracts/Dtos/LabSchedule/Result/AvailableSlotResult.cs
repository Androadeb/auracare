using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alexapps.SkinCare.Dtos.LabSchedule.Result
{
    public class AvailableSlotResult
    {
        public string TimeSlot { get; set; } 
        public bool IsAvailable { get; set; } 
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alexapps.SkinCare.Dtos.DiagnosticSessionTest.Result
{
    public class TimeSlotDto
    {
        public string TimeLabel { get; set; } // "10:00 AM"
        public DateTime ActualDateTime { get; set; }
        public bool IsAvailable { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alexapps.SkinCare.Dtos.Doctors.Results
{
    public class CreateRoomForSessionDto
    {
        public Guid DiagnosticSessionId { get; set; }
        public DateTime ScheduledStartTime { get; set; } 
    }
}

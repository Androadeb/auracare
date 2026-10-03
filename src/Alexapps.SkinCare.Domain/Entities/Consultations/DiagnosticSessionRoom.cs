using Alexapps.SkinCare.Entities.Base;
using Alexapps.SkinCare.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alexapps.SkinCare.Entities.Consultations
{
    public class DiagnosticSessionRoom : BaseEntity
    {
        public Guid DiagnosticSessionId { get; set; }
        public DiagnosticSession DiagnosticSession { get; set; }


        public string VideoRoomId { get; set; }
        public string Token { get; set; }
        public VideoRoomStatus Status { get; set; }

        public string DoctorJoinUrl { get; set; }
        public string PatientJoinUrl { get; set; }

      
        public DateTime ScheduledStartTime { get; set; }
        public DateTime ScheduledEndTime { get; set; } 

    
        public DateTime? ActualStartTime { get; set; }
    }
}

using Alexapps.SkinCare.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace Alexapps.SkinCare.Dtos.DiagnosticSessions.Results
{
    public class DiagnosticSessionRoomDto : EntityDto<Guid>
    {
       
        public Guid DiagnosticSessionId { get; set; }

       
        public string VideoRoomId { get; set; }

       
        public VideoRoomStatus Status { get; set; }

     
        public DateTime ScheduledStartTime { get; set; }
        public DateTime ScheduledEndTime { get; set; }

   
        public string DoctorJoinUrl { get; set; }
        public string PatientJoinUrl { get; set; }
        public string Token { get; set; }
      
        public double DurationMinutes => (ScheduledEndTime - ScheduledStartTime).TotalMinutes;

  
        //public bool IsLive => Status == VideoRoomStatus.AppointmentReserved &&
        //                     DateTime.Now >= ScheduledStartTime.AddMinutes(-10) &&
        //                     DateTime.Now <= ScheduledEndTime;

       
        public string StatusText => Status switch
        {
            VideoRoomStatus.AppointmentReserved => "Confirmed",
            VideoRoomStatus.Started => "Live Now",
            VideoRoomStatus.Ended => "Finished",
            VideoRoomStatus.Cancelled => "Cancelled",
            _ => "Unknown"
        };
    }
}

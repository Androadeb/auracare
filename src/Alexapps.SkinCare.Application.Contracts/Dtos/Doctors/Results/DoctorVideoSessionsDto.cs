using Alexapps.SkinCare.Dtos.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alexapps.SkinCare.Dtos.Doctors.Results
{
    public class DoctorVideoSessionsDto
    {
        public int TodayCallsCount { get; set; }
        public int WeeklySessionsCount { get; set; }
        public string AvgDuration { get; set; }

        public PagedResultWithMetadata<VideoSessionSlotDto> Appointments { get; set; }
    }

    public class VideoSessionSlotDto
    {
        public Guid SessionId { get; set; }
        public string PatientName { get; set; }
        public string PatientImageUrl { get; set; }
        public string ConsultationType { get; set; }
        public DateTime ScheduledStartTime { get; set; }
        public int DurationMinutes { get; set; }
        public bool CanStart { get; set; }
        public bool IsCompleted { get; set; }
        public string RoomId { get; set; }

        public string Token { get; set; }
    }
}

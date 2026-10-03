using Alexapps.SkinCare.Enums;
using System;

namespace Alexapps.SkinCare.Dtos.HomeServiceSessions.Queries
{
    public class GetHomeServiceSessionsInput
    {
        public int Page { get; set; } = 1;
        public int Limit { get; set; } = 10;
        public HomeServiceSessionStatus? Status { get; set; }
        public Guid? CustomerId { get; set; }
        public Guid? DoctorId { get; set; }
        public Guid? HomeServiceId { get; set; }
    }
}

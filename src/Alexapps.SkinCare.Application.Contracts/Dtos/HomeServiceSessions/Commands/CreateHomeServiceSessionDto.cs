using System;

namespace Alexapps.SkinCare.Dtos.HomeServiceSessions.Commands
{
    public class CreateHomeServiceSessionDto
    {
        public Guid HomeServiceId { get; set; }
        public Guid DoctorId { get; set; }
        public DateTime SessionDate { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Address { get; set; }
        public string? PhoneNumber { get; set; }
    }
}

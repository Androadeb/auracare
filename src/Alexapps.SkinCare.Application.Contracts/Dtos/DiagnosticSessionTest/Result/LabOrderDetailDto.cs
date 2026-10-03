using Alexapps.SkinCare.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alexapps.SkinCare.Dtos.DiagnosticSessionTest.Result
{
    public class LabOrderDetailDto
    {
        public Guid Id { get; set; }
        public string NumberOrder { get; set; }
        public string TestName { get; set; }
        public string Status { get; set; }

        // Patient Info (Left Card in image_c7115c)
        public string PatientName { get; set; }
        public string? PatientImage { get; set; }
        public string PatientGender { get; set; }
        public string PatientEmail { get; set; }
        public string PatientPhone { get; set; }
        public string PatientAddress { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public LabServiceType ServiceType { get; set; }
        public DateTime? DateOfBirth { get; set; }
        // Doctor Info (Right Card in image_c7115c)
        public string DoctorName { get; set; }
        public string Specialization { get; set; }
        public string DoctorContact { get; set; }
        public string? ResultFileName { get; set; }
        public string? ResultFileUrl { get; set; }
    }
}

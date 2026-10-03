using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alexapps.SkinCare.Dtos.DiagnosticSessionTest.Result
{
    public class DoctorOrderDetailDto
    {
        public Guid Id { get; set; }
        public Guid DiagnosticSessionId { get; set; }
      
        public string PatientName { get; set; }
        public string PatientGender { get; set; }
        public string PatientEmail { get; set; }
        public string PatientPhone { get; set; }

        public string? PatientImage { get; set; }
        public string MedicalHistorySummary { get; set; }

        // Test Info (Right Card in image_c70e1a)
        public string TestType { get; set; }
        public string Category { get; set; }
        public string SampleType { get; set; }

        // Assigned Lab
        public string LabName { get; set; }
        public string? ResultFileName { get; set; }
        public string? ResultFileUrl { get; set; }
    }
}

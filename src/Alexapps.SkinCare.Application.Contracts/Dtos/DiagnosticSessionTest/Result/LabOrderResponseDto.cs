using Alexapps.SkinCare.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alexapps.SkinCare.Dtos.DiagnosticSessionTest.Result
{
    public class LabOrderResponseDto
    {
        public Guid Id { get; set; }
        public string NumberOrder { get; set; } // This is our BookingCode
        public string DoctorName { get; set; }
        public string ServiceType { get; set; }
        public string MedicalTestName { get; set; }
        public string PatientName { get; set; }
        public DateTime? Date { get; set; }
        public string Status { get; set; }
      
    }
}

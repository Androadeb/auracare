using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alexapps.SkinCare.Dtos.DiagnosticSessionTest.Result
{
    public class DoctorOrderResponseDto
    {
        public Guid Id { get; set; }
        public string PatientName { get; set; }

        public string? PatientImage { get; set; }
        public string TestType { get; set; }
        public DateTime? RequestedDate { get; set; }
        public string Status { get; set; } 
    }
}

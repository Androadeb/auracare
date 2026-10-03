using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alexapps.SkinCare.Dtos.DiagnosticSessions.Results
{
    public class RoomValidationResultDto
    {
        public bool IsValid { get; set; }
        public string Message { get; set; }
        public string ErrorCode { get; set; } 
    }
}

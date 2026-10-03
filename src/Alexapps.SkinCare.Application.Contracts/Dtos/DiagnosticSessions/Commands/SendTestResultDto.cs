using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alexapps.SkinCare.Dtos.DiagnosticSessions.Commands
{
    public class SendTestResultDto
    {
        public Guid DiagnosticSessionTestId { get; set; }
        public Microsoft.AspNetCore.Http.IFormFile ResultFile { get; set; }
        public string? ResultFileName { get; set; }
    }
}

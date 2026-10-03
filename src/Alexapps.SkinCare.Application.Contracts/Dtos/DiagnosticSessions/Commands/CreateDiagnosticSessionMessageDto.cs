using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace Alexapps.SkinCare.Dtos.DiagnosticSessions.Commands
{
    public class CreateDiagnosticSessionMessageDto
    {
        public Guid DiagnosticSessionId { get; set; }
        public string Message { get; set; }
        public IFormFile File { get; set; }
    }
}

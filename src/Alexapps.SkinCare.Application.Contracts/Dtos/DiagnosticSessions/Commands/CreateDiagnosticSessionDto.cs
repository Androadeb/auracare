using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace Alexapps.SkinCare.Dtos.DiagnosticSessions.Commands
{
    public class CreateDiagnosticSessionDto
    {
        public Guid? DoctorId { get; set; }
        public string Description { get; set; }
        public string Duration { get; set; }
        public string ProductsUsed { get; set; }
        public string MedicalHistory { get; set; }
        public string Allergies { get; set; }
        public DateTime? DateOfBirth { get; set; }
        
        public List<IFormFile> Images { get; set; } = new List<IFormFile>();
    }
}

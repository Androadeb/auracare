using System;

namespace Alexapps.SkinCare.Dtos.DiagnosticSessions.Commands
{
    public class CreateDiagnosticSessionTestDto
    {
        public Guid DiagnosticSessionId { get; set; }
        public Guid MedicalTestId { get; set; }
        public Guid SampleTypeId { get; set; }
        public string PreparationInstructions { get; set; }
    }
}

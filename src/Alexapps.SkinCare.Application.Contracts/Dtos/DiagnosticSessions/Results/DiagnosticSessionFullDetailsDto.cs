using Alexapps.SkinCare.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alexapps.SkinCare.Dtos.DiagnosticSessions.Results
{
    public class DiagnosticSessionFullDetailsDto
    {
        public Guid Id { get; set; }
        public int TotalTestsCount { get; set; }
        public List<DiagnosticSessionFullTestDto> Tests { get; set; } = new();
        public List<DiagnosticSessionMessageTreatmentPlanDto> TreatmentPlans { get; set; } = new();
        public List<DiagnosticSessionFileDto> SessionFiles { get; set; } = new();
    }

    public class DiagnosticSessionFileDto
    {
        public Guid MessageId { get; set; }
        public string FileName { get; set; }
        public string FileUrl { get; set; }
        public DiagnosticSessionMessageType Type { get; set; }
        public DateTime CreationTime { get; set; }
    }
}

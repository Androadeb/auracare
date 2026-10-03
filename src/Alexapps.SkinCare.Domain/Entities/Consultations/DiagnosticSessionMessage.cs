using Alexapps.SkinCare.Entities.Base;
using Alexapps.SkinCare.Enums;
using System;
using System.Collections.Generic;

namespace Alexapps.SkinCare.Entities.Consultations
{
    public class DiagnosticSessionMessage : BaseEntity
    {
        public Guid DiagnosticSessionId { get; set; }
        public DiagnosticSession DiagnosticSession { get; set; }
        
        public Guid SenderId { get; set; }
        public string Message { get; set; }
        public DiagnosticSessionMessageType Type { get; set; }
        public bool IsRead { get; set; } = false;
        public string Token { get; set; }
        public string RoomId { get; set; }
        public string FileUrl { get; set; }
      
        public Guid? DiagnosticSessionTestId { get; set; }
        public DiagnosticSessionTest DiagnosticSessionTest { get; set; }

        public virtual ICollection<DiagnosticSessionTreatmentPlan> DiagnosticSessionTreatmentPlans { get; set; } = new List<DiagnosticSessionTreatmentPlan>();
    }
}

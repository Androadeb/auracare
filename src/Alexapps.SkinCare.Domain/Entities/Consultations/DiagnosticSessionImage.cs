using Alexapps.SkinCare.Entities.Base;
using System;

namespace Alexapps.SkinCare.Entities.Consultations
{
    public class DiagnosticSessionImage : BaseEntity
    {
        public Guid DiagnosticSessionId { get; set; }
        public DiagnosticSession DiagnosticSession { get; set; }
        public string ImageUrl { get; set; }
    }
}

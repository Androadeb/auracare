using System;
using Volo.Abp.Application.Dtos;

namespace Alexapps.SkinCare.Dtos.DiagnosticSessions.Results
{
    public class DiagnosticSessionImageDto : EntityDto<Guid>
    {
        public Guid DiagnosticSessionId { get; set; }
        public string ImageUrl { get; set; }
    }
}

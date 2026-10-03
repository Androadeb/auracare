
using System;
using Volo.Abp.Application.Dtos;


namespace Alexapps.SkinCare.Dtos.DiagnosticSessions.Queries
{
    public class GetDiagnosticSessionMessagesDto
    {
        public int Page { get; set; } = 1;
        public int Limit { get; set; } = 10;
        public Guid DiagnosticSessionId { get; set; }
    }
}

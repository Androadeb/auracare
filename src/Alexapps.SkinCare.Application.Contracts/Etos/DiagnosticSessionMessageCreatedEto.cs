using System;
using Volo.Abp.EventBus;
using Alexapps.SkinCare.Dtos.DiagnosticSessions.Results;

namespace Alexapps.SkinCare.Etos
{
    [EventName("Alexapps.SkinCare.DiagnosticSessionMessageCreated")]
    public class DiagnosticSessionMessageCreatedEto
    {
        public Guid SessionId { get; set; }
        public DiagnosticSessionMessageDto Message { get; set; }

        public DiagnosticSessionMessageCreatedEto()
        {
        }

        public DiagnosticSessionMessageCreatedEto(Guid sessionId, DiagnosticSessionMessageDto message)
        {
            SessionId = sessionId;
            Message = message;
        }
    }
}

using Alexapps.SkinCare.Dtos.DiagnosticSessions.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Alexapps.SkinCare.Interfaces
{
    public interface IDiagnosticSessionRoomAppService : IApplicationService
    {
    
        Task<DiagnosticSessionRoomDto> CreateMeetingRoomAsync(Guid diagnosticSessionId, DateTime startTime);

        Task<RoomValidationResultDto> ValidateRoomAsync(string roomId);

        Task SendBookingConfirmationAsync(Guid sessionId, DateTime scheduledTime);
        Task SendVideoCallLinkToChatAsync(Guid sessionId, string roomId, DateTime scheduledTime);
        Task EndActiveSessionAsync(string roomId);
        Task AutoEndRoomInDbAsync(string roomId);
    }
}

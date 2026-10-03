using Alexapps.SkinCare.Dtos.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Alexapps.SkinCare.Interfaces
{
    public interface INotificationAppService : IApplicationService
    {
        // 1. Receives the Token, Platform, and Language from the device
        Task<string> RegisterDeviceAsync(RegisterDeviceDto input);

        // 2. Globally enables or disables notifications for the user's account
        Task ToggleNotificationsAsync(bool isEnabled);

        // 3. Retrieves a paged list of notifications for the current user
        Task<PagedResultDto<NotificationDto>> GetMyNotificationsAsync(GetNotificationsInputDto input);

        // 4. Marks a specific notification as read
        Task MarkAsReadAsync(Guid id);
        Task PublishNotificationAsync(Guid userId, string title, string body, string targetType = null, Guid? targetId = null, Dictionary<string, string> data = null);
        Task NotifyPatientTreatmentPlanCreatedAsync(Guid userId, Guid sessionId, Guid messageId);

        Task NotifyDoctorNewBookingAsync(Guid doctorUserId, Guid sessionId, string patientName);

        Task NotifyPatientMedicalTestRequestedAsync(Guid userId, Guid sessionId, Guid testId);

        Task NotifyNewChatMessageAsync(Guid receiverUserId, Guid senderUserId, string senderName, string messagePreview);

        Task NotifyVideoRoomReadyAsync(Guid userId, string roomId, string token, bool isDoctor);
        Task NotifyPatientTestResultUploadedAsync(Guid userId, Guid sessionId, Guid testId, string testName);
    }
}

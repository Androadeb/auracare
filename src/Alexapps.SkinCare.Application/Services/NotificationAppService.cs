using Alexapps.SkinCare.Dtos.Notifications;
using Alexapps.SkinCare.Entities.Notifications;
using Alexapps.SkinCare.Entities.Users;
using Alexapps.SkinCare.Enums;
using Alexapps.SkinCare.Interfaces;
using Alexapps.SkinCare.Localization;
using Alexapps.SkinCare.Notifications;
using FirebaseAdmin.Messaging;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.EventBus.Local;
using Volo.Abp.Users;
using NotificationEntity = Alexapps.SkinCare.Entities.Notifications.Notification;

namespace Alexapps.SkinCare.Services
{
    [Authorize]
    public class NotificationAppService : ApplicationService, INotificationAppService
    {
        private readonly IRepository<UserDevice, Guid> _userDeviceRepository;
        private readonly IRepository<NotificationEntity, Guid> _notificationRepository;
        private readonly IBackgroundJobManager _backgroundJobManager;
        private readonly ILocalEventBus _localEventBus;
        public NotificationAppService(
            IRepository<UserDevice, Guid> userDeviceRepository,
            IRepository<NotificationEntity, Guid> notificationRepository,
            IBackgroundJobManager backgroundJobManager,
            ILocalEventBus localEventBus)
        {
            _userDeviceRepository = userDeviceRepository;
            _notificationRepository = notificationRepository;
            _backgroundJobManager = backgroundJobManager;
            _localEventBus = localEventBus;
            LocalizationResource = typeof(SkinCareResource);
        }
        public async Task<string> RegisterDeviceAsync(RegisterDeviceDto input)
        {
            var userId = CurrentUser.GetId();

          
            var existingToken = await _userDeviceRepository.FirstOrDefaultAsync(x => x.Token == input.Token);
            if (existingToken != null)
            {
                await _userDeviceRepository.DeleteAsync(existingToken);
            }

         
            var userDevice = new UserDevice(
                GuidGenerator.Create(),
                userId,
                input.Token,
                input.Language,
                isActive: true
            );

            // 3. الحفظ في قاعدة البيانات فقط
            await _userDeviceRepository.InsertAsync(userDevice);

            return "Device registered successfully in local database.";
        }
        public async Task ToggleNotificationsAsync(bool isEnabled)
        {
            var userId = CurrentUser.GetId();
            var devices = await _userDeviceRepository.GetListAsync(x => x.UserId == userId);

            foreach (var device in devices)
            {
                device.IsActive = isEnabled;
                await _userDeviceRepository.UpdateAsync(device);
            }
        }

        public async Task<PagedResultDto<NotificationDto>> GetMyNotificationsAsync(GetNotificationsInputDto input)
        {
            var userId = CurrentUser.GetId();
            var queryable = await _notificationRepository.GetQueryableAsync();

            var query = queryable
                .Where(x => x.UserId == userId)
                .WhereIf(input.IsRead.HasValue, x => x.IsRead == input.IsRead);

            var totalCount = await AsyncExecuter.CountAsync(query);

            var notifications = await AsyncExecuter.ToListAsync(
                query.OrderByDescending(x => x.CreationTime)
                     .PageBy(input.SkipCount, input.MaxResultCount)
            );

            var notificationDtos = ObjectMapper.Map<List<NotificationEntity>, List<NotificationDto>>(notifications);

            foreach (var dto in notificationDtos)
            {
                dto.Title = L[dto.Title];
                dto.Body = L[dto.Body];
             
            }

           
            var unreadNotifications = notifications.Where(x => !x.IsRead).ToList();
            if (unreadNotifications.Any())
            {
                foreach (var n in unreadNotifications) { n.IsRead = true; }
                await _notificationRepository.UpdateManyAsync(unreadNotifications);
            }

            return new PagedResultDto<NotificationDto>(totalCount, notificationDtos);
        }
        public async Task MarkAsReadAsync(Guid id)
        {
            var notification = await _notificationRepository.GetAsync(id);

            if (notification.UserId != CurrentUser.GetId())
            {
                throw new UnauthorizedAccessException("You cannot mark this notification as read.");
            }

            notification.IsRead = true;
            await _notificationRepository.UpdateAsync(notification);
        }
        [AllowAnonymous]
        [RemoteService(false)]
        public virtual async Task PublishNotificationAsync(
    Guid userId,
    string title,
    string body,
    string targetType = null,
    Guid? targetId = null,
    Dictionary<string, string> data = null)
        {
            var notification = new NotificationEntity
            {
                UserId = userId,
                Title = title,
                Body = body,
                IsRead = false,
                TargetType = targetType,
                TargetId = targetId
            };

            await _notificationRepository.InsertAsync(notification);

            await _backgroundJobManager.EnqueueAsync(new SendNotificationArgs
            {
                UserId = userId,
                Title = title,
                Body = body,
                Data = data
            });
        }
        [AllowAnonymous]
        [RemoteService(false)]
        // 1. New Booking Notification (To Doctor)
        public async Task NotifyDoctorNewBookingAsync(Guid doctorUserId, Guid sessionId, string patientName)
        {
            var data = new Dictionary<string, string>
    {
        { "SessionId", sessionId.ToString() },
        { "Type", "NewBooking" }
    };

            // Using Localization keys: "NewBookingTitle", "NewBookingBody"
            await PublishNotificationAsync(
                doctorUserId,
                L["NewBookingTitle"].Value,
                L["NewBookingBody", patientName].Value, // Passing patient name as a parameter to the localized string
                "NewBooking",
                sessionId,
                data
            );
        }
        [AllowAnonymous]
        [RemoteService(false)]
        // 2. Treatment Plan Notification (To Patient)
        public async Task NotifyPatientTreatmentPlanCreatedAsync(Guid userId, Guid sessionId, Guid planId)
        {
            var data = new Dictionary<string, string>
    {
        { "SessionId", sessionId.ToString() },
        { "PlanId", planId.ToString() },
        { "Type", "TreatmentPlan" }
    };

            await PublishNotificationAsync(
                userId,
                L["TreatmentPlanTitle"].Value,
                L["TreatmentPlanBody"].Value,
                "TreatmentPlan",
                planId,
                data
            );
        }
        [AllowAnonymous]
        [RemoteService(false)]
        // 3. Medical Test Notification (To Patient)
        public async Task NotifyPatientMedicalTestRequestedAsync(Guid userId, Guid sessionId, Guid testId)
        {
            var data = new Dictionary<string, string>
    {
        { "SessionId", sessionId.ToString() },
        { "TestId", testId.ToString() },
        { "Type", "MedicalTest" }
    };

            await PublishNotificationAsync(
                userId,
                L["MedicalTestTitle"].Value,
                L["MedicalTestBody"].Value,
                "MedicalTest",
                testId,
                data
            );
        }
        [AllowAnonymous]
        [RemoteService(false)]
        // 4. New Chat Message Notification
        public async Task NotifyNewChatMessageAsync(Guid receiverUserId, Guid senderId, string senderName, string messagePreview)
        {
            var data = new Dictionary<string, string>
    {
        { "SenderId", senderId.ToString() },
        { "Type", "NewMessage" }
    };

            await PublishNotificationAsync(
                receiverUserId,
                L["NewMessageTitle"].Value,
                L["NewMessageBody", senderName, messagePreview].Value,
                "NewMessage",
                senderId,
                data
            );
        }
        [AllowAnonymous]
        [RemoteService(false)]
        // 5. Video Call Notification
        public async Task NotifyVideoRoomReadyAsync(Guid userId, string roomId, string token, bool isDoctor)
        {
            var data = new Dictionary<string, string>
    {
        { "RoomId", roomId },
        { "Token", token },
        { "Type", "VideoCall" }
    };

   
            string title = L["VideoCallTitle"].Value;
            string body = isDoctor ? L["VideoCallDoctorBody"].Value : L["VideoCallPatientBody"].Value;

            await PublishNotificationAsync(
                userId,
                title,
                body,
                "VideoCall",
                null,
                data
            );
        }
        [AllowAnonymous]
        [RemoteService(false)]
        // 6. Test Result Notification (To Patient)
        public async Task NotifyPatientTestResultUploadedAsync(Guid userId, Guid sessionId, Guid testId, string testName)
        {
            var data = new Dictionary<string, string>
    {
        { "SessionId", sessionId.ToString() },
        { "TestId", testId.ToString() },
        { "Type", "TestResult" }
    };

            await PublishNotificationAsync(
                userId,
                L["TestResultTitle"].Value,
                L["TestResultBody", testName].Value,
                "TestResult",
                testId,
                data
            );
        }

    }
}
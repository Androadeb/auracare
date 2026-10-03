using Alexapps.SkinCare.Dtos.Notifications;
using Alexapps.SkinCare.Interfaces;
using Alexapps.SkinCare.Notifications; // تأكد من استخدام الـ Namespace الصحيح لخدمتك
using Alexapps.SkinCare.Routes;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;

namespace Alexapps.SkinCare.Controllers;

[RemoteService(Name = "Notifications")]
[Route(ApiRoutes.Notifications.Base)]
public class NotificationController : AbpController
{
    private readonly INotificationAppService _notificationAppService;

    public NotificationController(INotificationAppService notificationAppService)
    {
        _notificationAppService = notificationAppService;
    }

    [HttpPost("register-device")]
    public virtual async Task<string> RegisterDeviceAsync(RegisterDeviceDto input)
    {
     
        return await _notificationAppService.RegisterDeviceAsync(input);
    }

    [HttpPut("toggle")]
    public virtual async Task ToggleNotificationsAsync(bool isEnabled)
    {
        // Globally enables or disables notifications for the user's account
        await _notificationAppService.ToggleNotificationsAsync(isEnabled);
    }

    [HttpGet]
    public virtual async Task<PagedResultDto<NotificationDto>> GetMyNotificationsAsync(GetNotificationsInputDto input)
    {
        // Retrieves a paged list of notifications for the current user
        return await _notificationAppService.GetMyNotificationsAsync(input);
    }

    [HttpPut("{id}/read")]
    public virtual async Task MarkAsReadAsync(Guid id)
    {
       
        await _notificationAppService.MarkAsReadAsync(id);
    }
}
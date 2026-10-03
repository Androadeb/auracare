using Alexapps.SkinCare.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Alexapps.SkinCare.Jobs
{
    public class VideoCallNotificationJob : ITransientDependency
    {
        private readonly IDiagnosticSessionRoomAppService _videoRoomAppService;

        public VideoCallNotificationJob(IDiagnosticSessionRoomAppService videoRoomAppService)
        {
            _videoRoomAppService = videoRoomAppService;
        }

       
        public async Task SendLinkAsync(Guid sessionId, string roomId, DateTime scheduledTime)
        {
            await _videoRoomAppService.SendVideoCallLinkToChatAsync(sessionId, roomId, scheduledTime);
        }
    }
}

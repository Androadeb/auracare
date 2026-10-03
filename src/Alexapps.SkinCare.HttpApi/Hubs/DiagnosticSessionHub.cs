using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.AspNetCore.SignalR;

namespace Alexapps.SkinCare.Hubs
{
    [Authorize]
    public class DiagnosticSessionHub : AbpHub
    {
        public async Task JoinSessionGroupAsync(Guid sessionId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, sessionId.ToString());
        }

        public async Task LeaveSessionGroupAsync(Guid sessionId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, sessionId.ToString());
        }
    }
}

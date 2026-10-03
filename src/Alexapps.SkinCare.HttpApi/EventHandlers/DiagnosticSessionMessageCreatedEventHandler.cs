using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus;
using Alexapps.SkinCare.Etos;
using Alexapps.SkinCare.Hubs;
using Alexapps.SkinCare.Dtos.DiagnosticSessions.Results;

namespace Alexapps.SkinCare.EventHandlers
{
    public class DiagnosticSessionMessageCreatedEventHandler : ILocalEventHandler<DiagnosticSessionMessageCreatedEto>, ITransientDependency
    {
        private readonly IHubContext<DiagnosticSessionHub> _hubContext;

        public DiagnosticSessionMessageCreatedEventHandler(IHubContext<DiagnosticSessionHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task HandleEventAsync(DiagnosticSessionMessageCreatedEto eventData)
        {
            var groupName = eventData.SessionId.ToString();
            await _hubContext.Clients.Group(groupName).SendAsync("ReceiveMessage", eventData.Message);
        }
    }
}

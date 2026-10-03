using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.SignalR.Client;
using ynex.Models.Chat;

namespace ynex.Services.Chat
{
    public interface IChatService
    {
        Task<List<ChatMessage>> GetChatMessagesAsync(string sessionId, string token);
        Task<ChatMessage?> SendChatMessageAsync(MultipartFormDataContent content, string token);
        HubConnection CreateHubConnection(string token);
        Task<List<DoctorScheduleDto>> GetSchedulesAsync(string token);

        Task<bool> UpdateSchedulesAsync(List<DoctorScheduleDto> schedules, string token);
    }
}

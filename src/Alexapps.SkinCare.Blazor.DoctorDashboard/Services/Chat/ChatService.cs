using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using System.Globalization;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using ynex.Models.Chat;

namespace ynex.Services.Chat
{
    public class ChatService : IChatService
    {
        private readonly HttpClient _http;

        public ChatService(HttpClient http)
        {
            _http = http;
        }

        // تحسين: جعل الميثود private وواضحة للهيدرز الخاصة بكل طلب
        private void PrepareHeaders(HttpRequestMessage request, string token)
        {
            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var currentLanguage = CultureInfo.CurrentCulture.Name;
            request.Headers.AcceptLanguage.Clear();
            request.Headers.AcceptLanguage.Add(new StringWithQualityHeaderValue(currentLanguage));
        }

        public async Task<List<ChatMessage>> GetChatMessagesAsync(string sessionId, string token)
        {
            var url = $"api/v1/doctor/diagnostic-sessions/{sessionId}/messages?MaxResultCount=100";

            // استخدام using لضمان تنظيف الـ Message من الذاكرة فوراً
            using var request = new HttpRequestMessage(HttpMethod.Get, url);

            PrepareHeaders(request, token);

            var response = await _http.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ChatResponseWrapper>();
                return result?.Items.OrderBy(m => m.CreationTime).ToList() ?? new();
            }
            return new();
        }

        public async Task<ChatMessage?> SendChatMessageAsync(MultipartFormDataContent content, string token)
        {
            var url = "api/v1/doctor/diagnostic-sessions/messages";

            using var request = new HttpRequestMessage(HttpMethod.Post, url) { Content = content };

            PrepareHeaders(request, token);

            var response = await _http.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<ChatMessage>();
            }

            var error = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Chat Send Failed: {error}");
            return null;
        }

        public HubConnection CreateHubConnection(string token)
        {
            // ملاحظة: تأكد من أن الـ URL هنا هو الصحيح للـ Production أو الـ Staging
            return new HubConnectionBuilder()
                .WithUrl("https://auraskin.runasp.net/signalr-hubs/diagnostic-sessions", options =>
                {
                    options.AccessTokenProvider = () => Task.FromResult<string?>(token);
                    options.Transports = HttpTransportType.WebSockets | HttpTransportType.LongPolling;
                })
                .WithAutomaticReconnect()
                .Build();
        }
        public async Task<List<DoctorScheduleDto>> GetSchedulesAsync(string token)
        {
            try
            {
                var url = "api/v1/doctor/my-schedules";
                using var request = new HttpRequestMessage(HttpMethod.Get, url);

                // مهم جداً: إرسال التوكن واللغة
                PrepareHeaders(request, token);

                var response = await _http.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<List<DoctorScheduleDto>>() ?? new();
                }
                return new List<DoctorScheduleDto>();
            }
            catch (Exception)
            {
                return new List<DoctorScheduleDto>();
            }
        }

        // Update the list of schedules
        public async Task<bool> UpdateSchedulesAsync(List<DoctorScheduleDto> schedules, string token)
        {
            var url = "api/v1/doctor/my-schedules";

            // إنشاء الطلب يدوياً لاستخدام PrepareHeaders
            using var request = new HttpRequestMessage(HttpMethod.Put, url);
            request.Content = JsonContent.Create(schedules);

            PrepareHeaders(request, token); // إضافة Bearer Token واللغة

            var response = await _http.SendAsync(request);
            return response.IsSuccessStatusCode;
        }
    }
}
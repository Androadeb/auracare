using ynex.Models.ScheduledVideo;

namespace ynex.Services.VideoSessionService
{
    public class VideoSessionService : IVideoSessionService
    {
        private readonly HttpClient _httpClient;

        public VideoSessionService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<VideoDashboardResponse> GetDashboardDataAsync(int page = 1, int limit = 10, bool? isCompleted = null)
        {
            // بناء الرابط مع إضافة بارامتر الفلتر إذا وجد
            var url = $"api/v1/doctor/video-sessions-dashboard?page={page}&limit={limit}";
            if (isCompleted.HasValue)
            {
                url += $"&isCompleted={isCompleted.Value.ToString().ToLower()}";
            }

            var response = await _httpClient.GetFromJsonAsync<VideoDashboardResponse>(url);

            if (response?.Appointments?.Items != null)
            {
                var baseUrl = _httpClient.BaseAddress?.ToString().TrimEnd('/');

                response.Appointments.Items = response.Appointments.Items
                    .Select(item => {
                        if (!string.IsNullOrEmpty(item.PatientImageUrl) && !item.PatientImageUrl.StartsWith("http"))
                        {
                            item.PatientImageUrl = $"{baseUrl}/{item.PatientImageUrl.TrimStart('/')}";
                        }
                        return item;
                    })
                    // الترتيب: الأقرب وقتاً أولاً
                    .OrderBy(c => c.ScheduledStartTime)
                    .ToList();
            }
            return response;
        }
    }
}

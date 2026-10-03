using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Globalization;
using ynex.Models.Patient;
using Microsoft.Extensions.Configuration;
using ynex.Models.Auth;

namespace ynex.Services.Patient
{
    public class PatientService : IPatientService
    {
        private readonly HttpClient _http;

        public PatientService(HttpClient http)
        {
            _http = http;
        }

      
        private void PrepareHeaders(HttpRequestMessage request, string token)
        {
            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            // إرسال اللغة الحالية لضمان استلام البيانات المترجمة من الـ API
            var currentLanguage = CultureInfo.CurrentCulture.Name;
            request.Headers.AcceptLanguage.Clear();
            request.Headers.AcceptLanguage.Add(new StringWithQualityHeaderValue(currentLanguage));
        }

        public async Task<PatientSessionItem?> GetPatientDetailsAsync(string id, string token)
        {
            var url = $"api/v1/doctor/diagnostic-sessions/{id}";

            // إنشاء طلب جديد منفصل
            using var request = new HttpRequestMessage(HttpMethod.Get, url);

            // تجهيز الهيدرز لهذا الطلب فقط
            PrepareHeaders(request, token);

            // إرسال الطلب
            var response = await _http.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<PatientSessionItem>();
            }

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                throw new UnauthorizedAccessException();
            }

            return null;
        }

        public async Task<AuthResponse?> RefreshTokenAsync(string refreshToken)
        {
            var url = "api/v1/auth/refresh-token";

            // في طلبات البوست البسيطة نستخدم الطريقة المباشرة لسرعة التنفيذ
            var response = await _http.PostAsJsonAsync(url, new { refreshToken });

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<AuthResponse>();
            }

            return null;
        }
        public async Task<bool> CompleteSessionAsync(string id, string token)
        {
            var url = $"api/v1/doctor/diagnostic-sessions/{id}/complete";
            using var request = new HttpRequestMessage(HttpMethod.Post, url);
            PrepareHeaders(request, token);

            var response = await _http.SendAsync(request);
            return response.IsSuccessStatusCode;
        }
    }
}
using System.Globalization;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using ynex.Models.Chat;
using ynex.Models.Treatment;

namespace ynex.Services.Treatment
{
    public class TreatmentService : ITreatmentService
    {
        private readonly HttpClient _http;

        public TreatmentService(HttpClient http) => _http = http;

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

        public async Task<List<MedicationItem>> GetMedicationsAsync(string token)
        {
            var url = "api/v1/medications";
            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            PrepareHeaders(request, token);

            var response = await _http.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<MedicationResponse>();
                return result?.Items ?? new();
            }
            return new();
        }

        public async Task<bool> AddTreatmentItemAsync(object postData, string token)
        {
            var url = "api/v1/doctor/diagnostic-sessions/add-treatment-item";
            using var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = JsonContent.Create(postData)
            };
            PrepareHeaders(request, token);

            var response = await _http.SendAsync(request);
            return response.IsSuccessStatusCode;
        }

        public async Task<List<TreatmentPlanDetail>> GetAddedItemsAsync(string sessionId, string token)
        {
           
            var url = $"api/v1/doctor/diagnostic-sessions/added-items/{sessionId}";
            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            PrepareHeaders(request, token);

            var response = await _http.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<TreatmentPlanDetail>>() ?? new();
            }
            return new();
        }

        public async Task<bool> DeleteTreatmentItemAsync(string itemId, string token)
        {
            var url = $"api/v1/doctor/diagnostic-sessions/delete-treatment-item/{itemId}";
            using var request = new HttpRequestMessage(HttpMethod.Delete, url);
            PrepareHeaders(request, token);

            var response = await _http.SendAsync(request);
            return response.IsSuccessStatusCode;
        }

        public async Task<ChatMessage?> SendTreatmentPlanToPatientAsync(string sessionId, string token)
        {
          
            var url = $"api/v1/doctor/diagnostic-sessions/{sessionId}/send-treatment-plan";

            using var request = new HttpRequestMessage(HttpMethod.Post, url);
            PrepareHeaders(request, token);

            var response = await _http.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<ChatMessage>();
            }
            return null;
        }
    }
}
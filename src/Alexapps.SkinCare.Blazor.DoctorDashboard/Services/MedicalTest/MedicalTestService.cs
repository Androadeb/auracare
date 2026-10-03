using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Globalization;
using Microsoft.AspNetCore.Components;
using ynex.Models.Auth;
using ynex.Models.Chat;
using ynex.Models.MedicalTest;
using ynex.Models.Sessions; // لضمان الوصول لـ SessionAuthResponse

namespace ynex.Services.MedicalTest
{
    public class MedicalTestService : IMedicalTestService
    {
        private readonly HttpClient _http;
        private readonly AuthState _authState;
        private readonly NavigationManager _nav;
        private static readonly SemaphoreSlim _semaphore = new(1, 1);

        public MedicalTestService(HttpClient http, AuthState authState, NavigationManager nav)
        {
            _http = http;
            _authState = authState;
            _nav = nav;
        }

        private void PrepareHeaders(HttpRequestMessage request)
        {
            // نستخدم التوكن مباشرة من AuthState المحقون
            if (!string.IsNullOrEmpty(_authState.AccessToken))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _authState.AccessToken);
            }

            var currentLanguage = CultureInfo.CurrentCulture.Name;
            request.Headers.AcceptLanguage.Clear();
            request.Headers.AcceptLanguage.Add(new StringWithQualityHeaderValue(currentLanguage));
        }

        private async Task<HttpResponseMessage> SendRequestWithRetry(string url, HttpMethod method, object? postData = null)
        {
            var request = new HttpRequestMessage(method, url);
            if (postData != null)
            {
                request.Content = JsonContent.Create(postData);
            }

            PrepareHeaders(request);
            var response = await _http.SendAsync(request);

            // التعامل مع انتهاء صلاحية التوكن (Unauthorized)
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                if (await HandleTokenRefresh())
                {
                    var retryRequest = new HttpRequestMessage(method, url);
                    if (postData != null)
                    {
                        retryRequest.Content = JsonContent.Create(postData);
                    }
                    PrepareHeaders(retryRequest);
                    return await _http.SendAsync(retryRequest);
                }

                _nav.NavigateTo("/auth/sign-in");
            }
            return response;
        }

        private async Task<bool> HandleTokenRefresh()
        {
            if (!await _semaphore.WaitAsync(0))
            {
                await _semaphore.WaitAsync();
                _semaphore.Release();
                return true;
            }

            try
            {
                var refreshUrl = "api/v1/auth/refresh-token";
                var response = await _http.PostAsJsonAsync(refreshUrl, new { refreshToken = _authState.RefreshToken });

                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadFromJsonAsync<SessionAuthResponse>();
                    if (data != null)
                    {
                        _authState.AccessToken = data.AccessToken;
                        _authState.RefreshToken = data.RefreshToken;
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Token Refresh Error in MedicalTestService: {ex.Message}");
                return false;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<List<MedicalTestOrder>> GetOrdersAsync()
        {
            var url = "api/v1/doctor/diagnostic-session-tests/orders";
            var response = await SendRequestWithRetry(url, HttpMethod.Get);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<MedicalTestOrderResponse>();
                return result?.items ?? new List<MedicalTestOrder>();
            }
            return new List<MedicalTestOrder>();
        }

        public async Task<List<MedicalTestOrder>> GetOrdersBySessionIdAsync(string sessionId)
        {
            var url = $"api/v1/doctor/diagnostic-session-tests/orders?DiagnosticSessionId={sessionId}";
            var response = await SendRequestWithRetry(url, HttpMethod.Get);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<MedicalTestOrderResponse>();
                return result?.items ?? new List<MedicalTestOrder>();
            }
            return new List<MedicalTestOrder>();
        }

        public async Task<MedicalTestOrderDetail?> GetOrderDetailsAsync(string id)
        {
            var url = $"api/v1/doctor/diagnostic-session-tests/{id}/details";
            var response = await SendRequestWithRetry(url, HttpMethod.Get);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<MedicalTestOrderDetail>();
            }
            return null;
        }

        public async Task<List<MedicalCategory>> GetCategoriesAsync()
        {
            var url = "api/v1/medical-tests/categories";
            var response = await SendRequestWithRetry(url, HttpMethod.Get);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<MedicalCategory>>() ?? new();
            }
            return new();
        }

        public async Task<List<MedicalTestItem>> GetTestsByCategoryAsync(string categoryId)
        {
            var url = $"api/v1/medical-tests/categories/{categoryId}/tests";
            var response = await SendRequestWithRetry(url, HttpMethod.Get);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<MedicalTestItem>>() ?? new();
            }
            return new();
        }

        public async Task<List<SampleTypeItem>> GetSampleTypesByTestIdAsync(string medicalTestId)
        {
            var url = $"api/v1/medical-tests/sample-types?medicalTestId={medicalTestId}";
            var response = await SendRequestWithRetry(url, HttpMethod.Get);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<SampleTypeItem>>() ?? new();
            }
            return new();
        }

        public async Task<ChatMessage?> CreateMedicalTestAsync(object postData)
        {
            var url = "api/v1/doctor/diagnostic-sessions/tests";
            var response = await SendRequestWithRetry(url, HttpMethod.Post, postData);
            return response.IsSuccessStatusCode ? await response.Content.ReadFromJsonAsync<ChatMessage>() : null;
        }

        public async Task<bool> SendTestResultToChatAsync(string orderId, string fileUrl, string fileName)
        {
            var url = "api/v1/doctor/diagnostic-sessions/send-test-result";
            try
            {
                var fullUrl = fileUrl.StartsWith("http") ? fileUrl : $"https://auraskin.runasp.net{fileUrl}";
                byte[] fileBytes = await _http.GetByteArrayAsync(fullUrl);

                using var request = new HttpRequestMessage(HttpMethod.Post, url);
                var content = new MultipartFormDataContent();
                content.Add(new StringContent(orderId), "DiagnosticSessionTestId");

                var ms = new MemoryStream(fileBytes);
                var fileContent = new StreamContent(ms);
                fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
                content.Add(fileContent, "ResultFile", fileName);

                request.Content = content;
                PrepareHeaders(request);

                var response = await _http.SendAsync(request);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Service Error] SendTestResult: {ex.Message}");
                return false;
            }
        }
    }
}
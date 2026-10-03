using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Globalization;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using ynex.Models.Auth;
using ynex.Models.Sessions;

namespace ynex.Services.Sessions
{
    public class DiagnosticSessionService : IDiagnosticSessionService
    {
        private readonly HttpClient _http;
        private readonly NavigationManager _nav;
        private static readonly SemaphoreSlim _semaphore = new(1, 1);
        private readonly AuthState _authState;

        public DiagnosticSessionService(HttpClient http, NavigationManager nav, AuthState authState)
        {
            _http = http;
            _nav = nav;
            _authState = authState;
        }

        private void PrepareHeaders()
        {
            // تم إضافة التحقق من وجود التوكن لتجنب الأخطاء عند الرفع
            if (!string.IsNullOrEmpty(_authState.AccessToken))
            {
                _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authState.AccessToken);
            }

            var currentLanguage = CultureInfo.CurrentCulture.Name;
            _http.DefaultRequestHeaders.AcceptLanguage.Clear();
            _http.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue(currentLanguage));
        }

        public async Task<SessionResponse?> GetSessionsAsync(int page = 1, int limit = 10)
        {
            // بناء الرابط مع Query Parameters
            var url = $"api/v1/doctor/diagnostic-sessions?Page={page}&Limit={limit}";
            var response = await SendRequestWithRetry(url);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<SessionResponse>();
            }
            return null;
        }

        public async Task<SessionData?> GetSessionByIdAsync(string id)
        {
            var url = $"api/v1/doctor/diagnostic-sessions/{id}";
            var response = await SendRequestWithRetry(url);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<SessionData>();
            }
            return null;
        }

        private async Task<HttpResponseMessage> SendRequestWithRetry(string url)
        {
            PrepareHeaders();
            var response = await _http.GetAsync(url);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                if (await HandleTokenRefresh())
                {
                    PrepareHeaders();
                    return await _http.GetAsync(url);
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
                // مسار نسبي لتجديد التوكن
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
                Console.WriteLine($"Token Refresh Error: {ex.Message}");
                return false;
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }
}
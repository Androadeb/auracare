using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using ynex.Models.Auth;

namespace ynex.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly HttpClient _http;
        private readonly AuthenticationStateProvider _authStateProvider;
        private readonly ILocalStorageService _localStorage;

        public AuthService(
            HttpClient http,
            AuthenticationStateProvider authStateProvider,
            ILocalStorageService localStorage,
            IHttpClientFactory httpClientFactory)
        {
            _http = http;
            _authStateProvider = authStateProvider;
            _localStorage = localStorage;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<LoginResponse?> RefreshToken() // تم تغيير النوع المرجع هنا
        {
            var userInfo = await _localStorage.GetItemAsync<LoginResponse>("user_data");
            if (string.IsNullOrEmpty(userInfo?.RefreshToken)) return null;

            // استعارة العنوان من العميل المسجل ولكن إنشاء نسخة نظيفة (بدون Handlers)
            var baseClient = _httpClientFactory.CreateClient("SkinCareAPI");
            var cleanClient = _httpClientFactory.CreateClient();
            cleanClient.BaseAddress = baseClient.BaseAddress;

            var response = await cleanClient.PostAsJsonAsync("api/v1/auth/refresh-token", new { refreshToken = userInfo.RefreshToken });

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
                if (result != null)
                {
                    // تحديث البيانات داخلياً (بما فيها Local Storage) من خلال دالة النجاح
                    await HandleLoginSuccess(result);

                    // نرجع الكائن كاملاً للـ Handler
                    return result;
                }
            }

            await LogoutAsync();
            return null;
        }

        public async Task<LoginResponse?> SignInOtpAsync(LoginRequest request)
        {
            var response = await _http.PostAsJsonAsync("api/v1/auth/sign-in-otp", new
            {
                email = request.Email,
                password = request.Password
            });

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<LoginResponse>();
            }

            return null;
        }

        public async Task<LoginResponse?> VerifyOtpAsync(VerifyOtpRequest request)
        {
            var payload = new
            {
                username = request.UserName,
                otp = request.Otp,
                role = "DOCTOR"
            };

            var response = await _http.PostAsJsonAsync("api/v1/auth/verify-otp", payload);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
                if (result != null && !string.IsNullOrEmpty(result.AccessToken))
                {
                    await HandleLoginSuccess(result);
                }
                return result;
            }

            return null;
        }

        private async Task HandleLoginSuccess(LoginResponse response)
        {
            // Save full response to LocalStorage
            await _localStorage.SetItemAsync("user_data", response);

            // Update HttpClient Authorization header
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", response.AccessToken);

            // Notify AuthStateProvider
            if (_authStateProvider is CustomAuthStateProvider customProvider)
            {
                await customProvider.NotifyUserAuthentication(response);
            }
        }

        public async Task<bool> ResendOtpAsync(string username)
        {
            try
            {
                var payload = new { username = username, role = "DOCTOR" };

                using var request = new HttpRequestMessage(HttpMethod.Post, "api/v1/auth/send-otp");
                request.Headers.Add("Accept-Language", "ar-EG");
                request.Content = JsonContent.Create(payload);

                var response = await _http.SendAsync(request);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> SendPhoneOtpAsync(string phone)
        {
            try
            {
                var payload = new { username = phone, role = "DOCTOR" };
                var response = await _http.PostAsJsonAsync("api/v1/auth/send-otp", payload);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task LogoutAsync()
        {
            try
            {
                await _http.PostAsync("api/v1/auth/logout", null);
            }
            catch
            {
                // Ignore API logout failure during client cleanup
            }
            finally
            {
                await _localStorage.RemoveItemAsync("user_data");
                _http.DefaultRequestHeaders.Authorization = null;

                if (_authStateProvider is CustomAuthStateProvider customProvider)
                {
                    await customProvider.NotifyUserLogout();
                }
            }
        }
    }
}
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Globalization;
using Microsoft.AspNetCore.Components;
using ynex.Models.Auth;
using ynex.Models.Blog;
using ynex.Models.Sessions; // لضمان الوصول لـ SessionAuthResponse

namespace Alexapps.SkinCare.Blazor.Services
{
    public class BlogService : IBlogService
    {
        private readonly HttpClient _httpClient;
        private readonly AuthState _authState;
        private readonly NavigationManager _nav;
        private static readonly SemaphoreSlim _semaphore = new(1, 1);
        private const string BaseUrl = "api/v1/doctor/blogs";

        public BlogService(HttpClient httpClient, AuthState authState, NavigationManager nav)
        {
            _httpClient = httpClient;
            _authState = authState;
            _nav = nav;
        }

        private void PrepareHeaders(HttpRequestMessage request)
        {
            if (!string.IsNullOrEmpty(_authState.AccessToken))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _authState.AccessToken);
            }

            var currentLanguage = CultureInfo.CurrentCulture.Name;
            request.Headers.AcceptLanguage.Clear();
            request.Headers.AcceptLanguage.Add(new StringWithQualityHeaderValue(currentLanguage));
        }

        private async Task<HttpResponseMessage> SendRequestWithRetry(string url, HttpMethod method, HttpContent? content = null)
        {
            var request = new HttpRequestMessage(method, url) { Content = content };
            PrepareHeaders(request);

            var response = await _httpClient.SendAsync(request);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                if (await HandleTokenRefresh())
                {
                    // إعادة بناء المحتوى إذا كان Multipart لأنه لا يمكن إرساله مرتين من نفس الكائن
                    var retryRequest = new HttpRequestMessage(method, url) { Content = content };
                    PrepareHeaders(retryRequest);
                    return await _httpClient.SendAsync(retryRequest);
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
                var response = await _httpClient.PostAsJsonAsync(refreshUrl, new { refreshToken = _authState.RefreshToken });

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
            catch { return false; }
            finally { _semaphore.Release(); }
        }

        public async Task<BlogListResponse> GetBlogsAsync()
        {
            var response = await SendRequestWithRetry(BaseUrl, HttpMethod.Get);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<BlogListResponse>() ?? new();
            }
            return new();
        }

        public async Task<bool> CreateBlogAsync(MultipartFormDataContent content)
        {
            var response = await SendRequestWithRetry(BaseUrl, HttpMethod.Post, content);
            return response.IsSuccessStatusCode;
        }

        public async Task<BlogModel?> GetByIdAsync(Guid id)
        {
            var url = $"{BaseUrl}/{id}";
            var response = await SendRequestWithRetry(url, HttpMethod.Get);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<BlogModel>();
            }
            return null;
        }

        public async Task<bool> UpdateBlogAsync(Guid id, MultipartFormDataContent content)
        {
            var url = $"{BaseUrl}/{id}";
            var response = await SendRequestWithRetry(url, HttpMethod.Put, content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteBlogAsync(Guid id)
        {
            var url = $"{BaseUrl}/{id}";
            var response = await SendRequestWithRetry(url, HttpMethod.Delete);
            return response.IsSuccessStatusCode;
        }
    }
}
using Blazored.LocalStorage;
using System.Net;
using System.Net.Http.Headers;
using ynex.Models.Auth;

namespace ynex.Services.Auth
{
    public class RefreshTokenHandler : DelegatingHandler
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILocalStorageService _localStorage;

        public RefreshTokenHandler(IServiceProvider serviceProvider, ILocalStorageService localStorage)
        {
            _serviceProvider = serviceProvider;
            _localStorage = localStorage;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            try
            {
                // محاولة قراءة البيانات من LocalStorage
                // في مرحلة الـ Prerendering، سيفشل هذا السطر ويرمي Exception لأن JS غير متاح بعد
                var userInfo = await _localStorage.GetItemAsync<LoginResponse>("user_data");

                if (userInfo.AccessTokenExpireAt <= DateTime.UtcNow.AddMinutes(1))
                {
                    var authService = _serviceProvider.GetRequiredService<IAuthService>();

                    // تأكد أن دالة RefreshToken تعيد كائن LoginResponse كامل أو قم بتحديثه يدوياً
                    var loginData = await authService.RefreshToken();

                    if (loginData != null)
                    {
                        // 1. تحديث الهيدر للطلب الحالي
                        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", loginData.AccessToken);

                        // 2. تحديث المتصفح (السطر الذي كان ينقصك)
                        // هذا السطر سيجعل التاريخ يتغير من 2020 إلى 2026 في الـ Local Storage
                        await _localStorage.SetItemAsync("user_data", loginData);
                    }
                }
            }
            catch (Exception ex)
            {
                // إذا فشل الوصول إلى JS (مرحلة الـ Prerendering) 
                // نترك الطلب يمر كما هو دون إضافة Header، أو يمكنك تسجيل الخطأ للـ Debugging
                Console.WriteLine("Note: JS Interop not available during prerendering or LocalStorage failed.");
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
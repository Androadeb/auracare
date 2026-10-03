using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Localization;
using ynex.Models.Auth;
using ynex.Services.Auth;

namespace ynex.Pages.Auth
{
    public partial class VerifyOtp : ComponentBase, IDisposable
    {
        [Inject] private IAuthService AuthService { get; set; }
        [Inject] private NavigationManager Navigation { get; set; }
        [Inject] private AuthState AuthState { get; set; }
        [Inject] private AuthenticationStateProvider AuthStateProvider { get; set; }


        [Parameter][SupplyParameterFromQuery] public string? Username { get; set; }

        private string[] otpValues = new string[4] { "", "", "", "" };
        private int timeLeft = 60;
        private System.Threading.Timer? timer;
        private string? apiErrorMessage;
        private bool isProcessing = false;

        private string TimerDisplay => $"{timeLeft / 60:D2}:{timeLeft % 60:D2}";
        private bool IsResendDisabled => timeLeft > 0;

        protected override void OnInitialized()
        {
            StartTimer();
        }

        private void StartTimer()
        {
            timeLeft = 60;
            timer?.Dispose();
            timer = new System.Threading.Timer((_) =>
            {
                if (timeLeft > 0)
                {
                    timeLeft--;
                    InvokeAsync(StateHasChanged);
                }
            }, null, 0, 1000);
        }

        private async Task VerifyCode()
        {
            string fullOtp = string.Join("", otpValues).Trim();
            apiErrorMessage = null;

            if (fullOtp.Length < 4)
            {
                apiErrorMessage = L["Please fill in all required fields"];
                return;
            }

            if (isProcessing) return;

            isProcessing = true;
            try
            {
                var request = new VerifyOtpRequest
                {
                    UserName = Username ?? "",
                    Otp = fullOtp
                };

                // الـ AuthService هنا سيقوم بـ:
                // 1. طلب الـ API
                // 2. حفظ الـ AccessToken والـ RefreshToken في الـ LocalStorage
                // 3. إبلاغ الـ Provider بتغيير حالة المستخدم
                var result = await AuthService.VerifyOtpAsync(request);

                if (result != null)
                {
                    // لا حاجة لتعيين AccessToken يدوياً هنا ولا حاجة لاستدعاء NotifyUserAuthentication
                    // لأن الخدمة قامت بذلك بالفعل. فقط توجه لصفحة التحكم.
                    Navigation.NavigateTo("/admin/dashboard");
                }
                else
                {
                    apiErrorMessage = L["InvalidOtpCode"];
                    otpValues = new string[4] { "", "", "", "" };
                }
            }
            catch (Exception ex)
            {
                apiErrorMessage = L["ConnectionError"];
                Console.WriteLine($"OTP Verification Exception: {ex.Message}");
            }
            finally
            {
                isProcessing = false;
                await InvokeAsync(StateHasChanged);
            }
        }

        private async Task ResendCode()
        {
            if (IsResendDisabled || string.IsNullOrEmpty(Username)) return;

            try
            {
           
                var success = await AuthService.SendPhoneOtpAsync(Username);

                if (success)
                {
                    apiErrorMessage = L["OtpResentSuccess"];
                    StartTimer();
                }
                else
                {
                    apiErrorMessage = L["OtpResentFailed"];
                }
            }
            catch (Exception)
            {
                apiErrorMessage = L["ConnectionError"];
            }
        }

        private string MaskEmail(string email)
        {
            if (string.IsNullOrEmpty(email) || !email.Contains("@")) return email;
            var parts = email.Split('@');
            return parts[0].Length <= 2 ? email : $"{parts[0].Substring(0, 2)}***@{parts[1]}";
        }

        public void Dispose() => timer?.Dispose();
    }
}
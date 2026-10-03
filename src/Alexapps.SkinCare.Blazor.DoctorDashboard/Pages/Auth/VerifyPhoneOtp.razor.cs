using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using ynex.Models.Auth;
using ynex.Services.Auth;
using AlexApps.Classat.Blazor.Superadmin.Services;

namespace ynex.Pages.Auth
{
    public partial class VerifyPhoneOtp : ComponentBase, IDisposable
    {
        [Inject] private IAuthService AuthService { get; set; }
        [Inject] private NavigationManager Navigation { get; set; }
      
     
        [Inject] private AuthenticationStateProvider AuthStateProvider { get; set; }

        [Parameter][SupplyParameterFromQuery(Name = "phone")] public string? PhoneNumber { get; set; }

        private string[] otpValues = new string[4] { "", "", "", "" };
        private int timeLeft = 60;
        private System.Threading.Timer? timer;
        private string? apiErrorMessage;
        private bool isProcessing = false;

        private string TimerDisplay => $"{timeLeft / 60:D2}:{timeLeft % 60:D2}";
        private bool IsResendDisabled => timeLeft > 0;

        protected override void OnInitialized() => StartTimer();

        private void StartTimer()
        {
            timeLeft = 60;
            timer?.Dispose();
            timer = new System.Threading.Timer((_) => {
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

            if (fullOtp.Length < 4)
            {
                apiErrorMessage = L["Please fill in all required fields"];
                return;
            }

            if (isProcessing) return;

            isProcessing = true;
            apiErrorMessage = null;

            try
            {
                var result = await AuthService.VerifyOtpAsync(new VerifyOtpRequest
                {
                    UserName = PhoneNumber ?? "",
                    Otp = fullOtp
                });

                if (result != null)
                {
                    // ملاحظة: لا حاجة لاستدعاء NotifyUserAuthentication هنا 
                    // لأن AuthService.VerifyOtpAsync يقوم بذلك داخلياً الآن.

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
                Console.WriteLine($"Verify OTP Error: {ex.Message}");
            }
            finally
            {
                isProcessing = false;
                await InvokeAsync(StateHasChanged);
            }
        }

        private async Task ResendCode()
        {
            if (IsResendDisabled || string.IsNullOrEmpty(PhoneNumber)) return;

            try
            {
               
                var success = await AuthService.SendPhoneOtpAsync(PhoneNumber);

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
            catch (Exception ex)
            {
                apiErrorMessage = L["ConnectionError"];
                Console.WriteLine($"Resend Phone Error: {ex.Message}");
            }
        }

        private string MaskPhone(string? phone)
        {
            if (string.IsNullOrEmpty(phone) || phone.Length < 8) return phone ?? "";
            return $"{phone.Substring(0, 4)}****{phone.Substring(phone.Length - 3)}";
        }

        public void Dispose() => timer?.Dispose();
    }
}
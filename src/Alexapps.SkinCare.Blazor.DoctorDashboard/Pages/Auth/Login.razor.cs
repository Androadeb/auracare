using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.ComponentModel.DataAnnotations;
using ynex.Models.Auth;
using ynex.Services.Auth;
using CountryLib = CountryData.Standard;

namespace ynex.Pages.Auth
{
    public partial class Login
    {
        [Inject] private NavigationManager Navigation { get; set; }
        [Inject] private IAuthService AuthServiceInstance { get; set; }

        private bool showEmailInput = true;
        private bool isSubmitting = false;

        private EmailLoginRequest emailModel = new EmailLoginRequest();
        private PhoneLoginRequest phoneModel = new PhoneLoginRequest();

        private EditContext emailContext;
        private EditContext phoneContext;

        private string errorMessage = "";

        // --- متغيرات الدول الجديدة ---
        private IEnumerable<CountryLib.Country> allCountries = new List<CountryLib.Country>();
        private string selectedPhoneCode = "971"; // الافتراضي
        private string selectedCountryIso = "AE";
        // ---------------------------

        protected override void OnInitialized()
        {
            emailContext = new EditContext(emailModel);
            phoneContext = new EditContext(phoneModel);

            // تحميل بيانات الدول
            try
            {
                var helper = new CountryLib.CountryHelper();
                allCountries = helper.GetCountryData().OrderBy(c => c.CountryName).ToList();
            }
            catch { }
        }

        private void OnCountryChange(ChangeEventArgs e)
        {
            var iso = e.Value?.ToString();
            var country = allCountries.FirstOrDefault(c => c.CountryShortCode == iso);
            if (country != null)
            {
                selectedCountryIso = country.CountryShortCode;
             
                selectedPhoneCode = country.PhoneCode.Replace("+", "").Trim();
            }
        }

        private void SwitchToEmail()
        {
            showEmailInput = true;
            errorMessage = string.Empty;
            emailModel = new EmailLoginRequest();
            emailContext = new EditContext(emailModel);
        }

        private void SwitchToPhone()
        {
            showEmailInput = false;
            errorMessage = string.Empty;
            phoneModel = new PhoneLoginRequest();
            phoneContext = new EditContext(phoneModel);
        }

        private async Task HandleEmailLogin()
        {
            errorMessage = "";
            if (!emailContext.Validate()) return;

            isSubmitting = true;
            try
            {
                var result = await AuthServiceInstance.SignInOtpAsync(new LoginRequest
                {
                    Email = emailModel.Email,
                    Password = emailModel.Password
                });

                if (result != null)
                {
                    Navigation.NavigateTo($"/auth/verify-code?username={Uri.EscapeDataString(emailModel.Email)}");
                }
                else
                {
                    errorMessage = L["InvalidLoginAttempt"];
                }
            }
            catch (Exception ex)
            {
                errorMessage = L["ConnectionError"];
                Console.WriteLine(ex.Message);
            }
            finally
            {
                isSubmitting = false;
                StateHasChanged();
            }
        }

        private async Task HandlePhoneLogin()
        {
            errorMessage = "";
            if (!phoneContext.Validate()) return;

            isSubmitting = true;
            try
            {
              
                string cleanCountryCode = selectedPhoneCode?.Replace("+", "").Trim() ?? "";

              
                string rawPhone = phoneModel.Phone?.Trim() ?? "";

              
                if (rawPhone.StartsWith("0"))
                {
                    rawPhone = rawPhone.Substring(1);
                }


                var fullPhoneNumber = $"+{cleanCountryCode}{rawPhone}";


                var success = await AuthServiceInstance.SendPhoneOtpAsync(fullPhoneNumber);

                if (success)
                {
                  
                    Navigation.NavigateTo($"/auth/verify-code?username={Uri.EscapeDataString(fullPhoneNumber)}");
                }
                else
                {
                    errorMessage = L["PhoneNotRegistered"];
                }
            }
            catch (Exception ex)
            {
                errorMessage = L["ConnectionError"];
                Console.WriteLine($"Login Error: {ex.Message}");
            }
            finally
            {
                isSubmitting = false;
                StateHasChanged();
            }
        }
    }
}
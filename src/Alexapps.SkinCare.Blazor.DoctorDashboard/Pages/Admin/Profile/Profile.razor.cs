using CountryData.Standard;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using ynex.Models.Profile;
using ynex.Services.Profile;
using CountryLib = CountryData.Standard;

namespace ynex.Pages.Admin.Profile
{
    public partial class Profile : ComponentBase
    {
        [Inject] private IProfileService ProfileService { get; set; } = default!;
        [Inject] private NavigationManager Navigation { get; set; } = default!;
        [Inject] private IJSRuntime JSRuntime { get; set; } = default!;

        public ProfileDto profileData = new();
        public QualificationDto currentQualification = new();
        private List<SpecialtyModel> Specialties = new();

        private List<CountryLib.Country> allCountries = new();
        private string? selectedCountryIso = "ae";
        private string selectedPhoneCode = "971";

        private string? certificatePreviewUrl;
        private IBrowserFile? profileImageFile;
        private string? previewImageUrl;
        private IBrowserFile? selectedFile;
        private bool isEditMode = false;
        private bool isLoading = true;
        private bool isQualificationModalOpen = false;
        private bool isOtpModalOpen = false;
        private string otpCode = "";
        private bool isOtpLoading = false;
        private string? otpErrorMessage;
        private string[] modalOtpValues = new string[4] { "", "", "", "" };
        private string originalPhoneNumber = "";
        private string originalEmail = "";
        private ProfileDto originalData = new();
        protected override async Task OnInitializedAsync()
        {
            InitializeCountryData();
            await LoadData();
        }

        private void InitializeCountryData()
        {
            var helper = new CountryHelper();
            allCountries = helper.GetCountryData()
                .OrderBy(c => c.CountryName)
                .ToList();
        }

        private async Task LoadData()
        {
            try
            {
                isLoading = true;
                Specialties = await ProfileService.GetSpecialtiesAsync();
                var data = await ProfileService.GetProfileAsync();
                if (data != null)
                {
                    // حفظ البيانات للعرض
                    profileData = data;

                    // حفظ القيم الأصلية بشكل مستقل للمقارنة في الواجهة (Razor)
                    originalPhoneNumber = data.PhoneNumber ?? "";
                    originalEmail = data.Email ?? "";

                    // حفظ نسخة للمقارنة عند الضغط على Save لمنع ظهور الـ Modal بالخطأ
                    originalData = new ProfileDto
                    {
                        FullName = data.FullName,
                        Email = data.Email,
                        PhoneNumber = data.PhoneNumber,
                        SpecialtyId = data.SpecialtyId,
                        DescriptionEn = data.DescriptionEn
                    };

                    ExtractCountryFromPhone();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error Loading Profile Data: {ex.Message}");
            }
            finally
            {
                isLoading = false;
                StateHasChanged();
            }
        }

        private void ExtractCountryFromPhone()
        {
            if (string.IsNullOrEmpty(profileData.PhoneNumber)) return;
            var match = allCountries.FirstOrDefault(c => profileData.PhoneNumber.StartsWith("+" + c.PhoneCode.Replace("+", "")));
            if (match != null)
            {
                selectedCountryIso = match.CountryShortCode;
                selectedPhoneCode = match.PhoneCode.Replace("+", "");
                profileData.PhoneNumber = profileData.PhoneNumber.Replace("+" + selectedPhoneCode, "");
            }
        }

        private void OnCountryChange(ChangeEventArgs e)
        {
            var iso = e.Value?.ToString();
            var country = allCountries.FirstOrDefault(c => c.CountryShortCode == iso);
            if (country != null)
            {
                selectedCountryIso = country.CountryShortCode;
                selectedPhoneCode = country.PhoneCode.Replace("+", "");
            }
        }

        private async Task SaveProfile()
        {
            // تحقق هل تغير شيء فعلاً؟
            bool hasChanges = profileData.FullName != originalData.FullName ||
                              profileData.Email != originalData.Email ||
                              profileData.PhoneNumber != originalData.PhoneNumber ||
                              profileData.SpecialtyId != originalData.SpecialtyId ||
                              profileData.DescriptionEn != originalData.DescriptionEn ||
                              profileImageFile != null;

            if (!hasChanges)
            {
                isEditMode = false;
                return;
            }

            try
            {
                isLoading = true;
                var result = await ProfileService.UpdateProfileAsync(profileData, profileImageFile);
                if (result != null)
                {

                    if (result.RequiresVerification)
                    {
                    
                        modalOtpValues = new string[4] { "", "", "", "" };
                        otpErrorMessage = null;
                        isOtpModalOpen = true;
                    }
                    else
                    {
                        isEditMode = false;
                        await LoadData();
                    }
                }
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
            finally { isLoading = false; }
        }

        private async Task HandleVerifyOtp()
        {
            otpCode = string.Join("", modalOtpValues);
            if (otpCode.Length < 4)
            {
                otpErrorMessage = L["Please enter the full 4-digit code"];
                return;
            }
            await VerifyOtpLogic();
        }

        private async Task VerifyOtpLogic()
        {
            isOtpLoading = true;
            try 
            {
                // افترضنا وجود ميثود Verify في السيرفس
                var success = await ProfileService.VerifyUpdateOtpAsync(otpCode);
                if (success)
                {
                    isOtpModalOpen = false;
                    isEditMode = false;
                    await LoadData();
                }
                else { otpErrorMessage = L["Invalid Code"]; }
            }
            finally { isOtpLoading = false; }
        }

        private string MaskEmail(string email)
        {
            if (string.IsNullOrEmpty(email) || !email.Contains("@")) return email;
            var parts = email.Split('@');
            return parts[0].Length <= 2 ? email : $"{parts[0].Substring(0, 2)}***@{parts[1]}";
        }

        private async Task HandleModalKeyUp(KeyboardEventArgs e, int index)
        {
            if (e.Key == "Backspace")
            {
                if (index > 0 && string.IsNullOrEmpty(modalOtpValues[index]))
                    await JSRuntime.InvokeVoidAsync("focusElement", $"modal-otp-{index - 1}");
            }
            else if (modalOtpValues[index].Length == 1 && index < 3)
            {
                await JSRuntime.InvokeVoidAsync("focusElement", $"modal-otp-{index + 1}");
            }
        }

        private void CloseOtpModal() { isOtpModalOpen = false; otpErrorMessage = null; }
        private void ToggleEdit() => isEditMode = true;
        private async Task CancelEdit() { isEditMode = false; await LoadData(); }

        // --- Qualification Handlers ---
        private void OpenAddQualificationModal()
        {
            currentQualification = new QualificationDto();
            selectedFile = null;
            certificatePreviewUrl = null;
            isQualificationModalOpen = true;
        }

        private void EditQualification(QualificationDto qual)
        {
            currentQualification = new QualificationDto { Id = qual.Id, Degree = qual.Degree, CertificateImageUrl = qual.CertificateImageUrl };
            isQualificationModalOpen = true;
        }

        private async Task HandleSaveQualification()
        {
            if (string.IsNullOrWhiteSpace(currentQualification.Degree)) return;
            var success = await ProfileService.UpsertQualificationAsync(currentQualification, selectedFile);
            if (success) { isQualificationModalOpen = false; await LoadData(); }
        }

        private async Task DeleteQualification(string id)
        {
            if (await ProfileService.DeleteQualificationAsync(id)) await LoadData();
        }

        private void CloseModal() => isQualificationModalOpen = false;

        private async Task HandleProfileImageSelected(InputFileChangeEventArgs e)
        {
            profileImageFile = e.File;
            previewImageUrl = await GetPreviewUrl(e.File);
        }

        private async Task HandleCertificateFileSelected(InputFileChangeEventArgs e)
        {
            selectedFile = e.File;
            certificatePreviewUrl = await GetPreviewUrl(e.File);
        }

        private async Task<string> GetPreviewUrl(IBrowserFile file)
        {
            var resized = await file.RequestImageFileAsync(file.ContentType, 300, 300);
            var buffer = new byte[resized.Size];
            using var stream = resized.OpenReadStream();
            await stream.ReadAsync(buffer);
            return $"data:{file.ContentType};base64,{Convert.ToBase64String(buffer)}";
        }
    }
}
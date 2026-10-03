using Microsoft.AspNetCore.Components.Forms;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using ynex.Models.Profile;

namespace ynex.Services.Profile
{
    public class ProfileService : IProfileService
    {
        private readonly HttpClient _http;

        public ProfileService(HttpClient http)
        {
            _http = http;
        }

       
        public async Task<List<SpecialtyModel>> GetSpecialtiesAsync()
        {
            try
            {
                var response = await _http.GetAsync("api/v1/doctors/specialties");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<List<SpecialtyModel>>() ?? new List<SpecialtyModel>();
                }
                return new List<SpecialtyModel>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching specialties: {ex.Message}");
                return new List<SpecialtyModel>();
            }
        }

        public async Task<ProfileDto?> GetProfileAsync()
        {
            try
            {
                var response = await _http.GetAsync("api/v1/auth/my-info");

                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return null;
                }

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<ProfileDto>();
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Service Layer Error: {ex.Message}");
                return null;
            }
        }

        // English comment: Update the method to return the DTO instead of a boolean
        public async Task<ProfileDto?> UpdateProfileAsync(ProfileDto profile, IBrowserFile? profileImage)
        {
            using var content = new MultipartFormDataContent();

            content.Add(new StringContent(profile.FullName ?? ""), "Name");
            content.Add(new StringContent(profile.PhoneNumber ?? ""), "PhoneNumber");
            content.Add(new StringContent(profile.Email ?? ""), "Email");
            content.Add(new StringContent(profile.DescriptionEn ?? ""), "DescriptionEn");

            if (profile.SpecialtyId != null)
            {
                content.Add(new StringContent(profile.SpecialtyId.ToString()!), "SpecialtyId");
            }

            if (profileImage != null)
            {
                var fileContent = new StreamContent(profileImage.OpenReadStream(maxAllowedSize: 1024 * 1024 * 5));
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(profileImage.ContentType);
                content.Add(fileContent, "ProfileImage", profileImage.Name);
            }

            var response = await _http.PutAsync("api/v1/auth/my-info", content);

            if (response.IsSuccessStatusCode)
            {
                // English comment: Read the updated profile from the response body as you showed in the JSON examples
                return await response.Content.ReadFromJsonAsync<ProfileDto>();
            }

            return null;
        }
        public async Task<bool> VerifyUpdateOtpAsync(string otpCode)
        {
            try
            {
                // English comment: Matching the schema from image_4778fc.png where request body is { "otp": "string" }
                var requestBody = new { otp = otpCode };
                var response = await _http.PostAsJsonAsync("api/v1/auth/verify-update-otp", requestBody);

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"OTP Verification Error: {ex.Message}");
                return false;
            }
        }
        public async Task<bool> UpsertQualificationAsync(QualificationDto qual, IBrowserFile? file)
        {
            using var content = new MultipartFormDataContent();
            content.Add(new StringContent(qual.Degree ?? ""), "Degree");

            if (file != null)
            {
                var fileContent = new StreamContent(file.OpenReadStream(maxAllowedSize: 1024 * 1024 * 5));
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
                content.Add(fileContent, "CertificateImage", file.Name);
            }

            HttpResponseMessage response;
            if (string.IsNullOrEmpty(qual.Id))
            {
                response = await _http.PostAsync("api/v1/doctor/qualifications", content);
            }
            else
            {
                response = await _http.PutAsync($"api/v1/doctor/qualifications/{qual.Id}", content);
            }

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteQualificationAsync(string id)
        {
            var response = await _http.DeleteAsync($"api/v1/doctor/qualifications/{id}");
            return response.IsSuccessStatusCode;
        }
    }

  
}
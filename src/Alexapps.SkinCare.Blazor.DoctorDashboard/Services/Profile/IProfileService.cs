using Microsoft.AspNetCore.Components.Forms;
using ynex.Models.Profile;

namespace ynex.Services.Profile
{
    public interface IProfileService
    {
        Task<ProfileDto?> GetProfileAsync();
        Task<bool> UpsertQualificationAsync(QualificationDto qual, IBrowserFile? file);
        Task<bool> DeleteQualificationAsync(string id);

        Task<ProfileDto?> UpdateProfileAsync(ProfileDto profile, IBrowserFile? profileImage);

        Task<bool> VerifyUpdateOtpAsync(string otpCode);
        Task<List<SpecialtyModel>> GetSpecialtiesAsync();
    }
}

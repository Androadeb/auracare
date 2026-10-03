using ynex.Models.Patient;

namespace ynex.Services.Patient
{
    public interface IPatientService
    {
        Task<PatientSessionItem?> GetPatientDetailsAsync(string id, string token);
        Task<AuthResponse?> RefreshTokenAsync(string refreshToken);
        Task<bool> CompleteSessionAsync(string id, string token);
    }
}

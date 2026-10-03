using System.Threading.Tasks;
using ynex.Models.Auth;

namespace ynex.Services.Auth
{
    public interface IAuthService
    {
        Task<LoginResponse?> RefreshToken();
        Task<LoginResponse?> SignInOtpAsync(LoginRequest request);
        Task<LoginResponse?> VerifyOtpAsync(VerifyOtpRequest request);
        Task<bool> ResendOtpAsync(string username);
        Task<bool> SendPhoneOtpAsync(string phone);
        Task LogoutAsync();
    }
}

using Alexapps.SkinCare.Business.Client.Profile.Results;
using Alexapps.SkinCare.Business.OTP.Commands.SendOtp;
using Alexapps.SkinCare.Business.OTP.Commands.VerifyOtp;
using Alexapps.SkinCare.Business.OTP.Results;
using Alexapps.SkinCare.Dtos.Auth.Commands.Login;
using Alexapps.SkinCare.Dtos.Auth.Commands.RefreshToken;
using Alexapps.SkinCare.Dtos.Auth.Commands.Register;
using Alexapps.SkinCare.Dtos.Auth.Commands.UpdateProfile;
using Alexapps.SkinCare.Dtos.Auth.Results.Login;
using Alexapps.SkinCare.Dtos.Auth.Results.Register;
using Alexapps.SkinCare.Dtos.OTP.Commands.VerifyOtp;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Alexapps.SkinCare.Interfaces;

public interface IAuthService : IApplicationService
{
    Task<string> GenerateTokenAsync(Guid userId, List<Claim> claims = null);

    Task<AuthResult> VerifyOTPAsync(VerifyOtpCommand input);

    Task<SendOtpResult> SendOTPAsync(SendOtpCommand input);

    Task<AuthResult> RefreshTokenAsync(RefreshTokenCommand refreshTokenCommand);

    Task<RegisterResult> RegisterAsync(RegisterCommand Command);

    Task<GetProfileResult> GetProfileAsync();

    Task<UpdateProfileResult> UpdateProfileAsync(UpdateProfileCommand command);
    Task<bool> VerifyUpdateOtpAsync(VerifyUpdateOtpCommand command);
    Task LogoutAsync();

    Task<SendOtpResult> SignInWithEmailAndSendOtpAsync(SignInWithEmailDto input);
}


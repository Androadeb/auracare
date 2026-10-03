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
using Alexapps.SkinCare.Interfaces;
using Alexapps.SkinCare.Routes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Account;

namespace Alexapps.SkinCare.Controllers;

public class AuthController(IAuthService authService) : SkinCareController
{

// 
    [AllowAnonymous]
    [HttpPost(ApiRoutes.Auth.SendOtp)]
    public async Task<SendOtpResult> SendOTPAsync([FromBody] SendOtpCommand sendOtpRequest)
    {
        return await authService.SendOTPAsync(sendOtpRequest);
    }

    [AllowAnonymous]
    [HttpPost(ApiRoutes.Auth.VerifyOtp)]
    public async Task<AuthResult> VerifyOTPAsync([FromBody] VerifyOtpCommand verifyOtpRequest)
    {
        return await authService.VerifyOTPAsync(verifyOtpRequest);
    }



    [HttpPost(ApiRoutes.Auth.RefreshToken)]
    public async Task<AuthResult> RefreshTokenAsync([FromBody] RefreshTokenCommand refreshTokenCommand)
    {
        return await authService.RefreshTokenAsync(refreshTokenCommand);
    }

    [Authorize]
    [HttpGet(ApiRoutes.Auth.MyInfo)]
    public async Task<GetProfileResult> GetProfileAsync()
    {
        return await authService.GetProfileAsync();
    }

    [Authorize]
    [HttpPut(ApiRoutes.Auth.MyInfo)]
    public async Task<UpdateProfileResult> UpdateProfileAsync([FromForm] UpdateProfileCommand command)
    {
        return await authService.UpdateProfileAsync(command);
    }
    [Authorize]
    [HttpPost(ApiRoutes.Auth.VerifyUpdateOtp)]
    public async Task<bool> VerifyUpdateOtpAsync([FromBody] VerifyUpdateOtpCommand command)
    {
        return await authService.VerifyUpdateOtpAsync(command);
    }
    [Authorize]
    [HttpPost(ApiRoutes.Auth.Logout)]
    public async Task LogoutAsync()
    {
        await authService.LogoutAsync();
    }
    [AllowAnonymous]
    [HttpPost(ApiRoutes.Auth.SignInWithEmailAndSendOtp)]
    public async Task<SendOtpResult> SignInWithEmailAndSendOtpAsync([FromBody] SignInWithEmailDto input)
    {
        return await authService.SignInWithEmailAndSendOtpAsync(input);
    }
}



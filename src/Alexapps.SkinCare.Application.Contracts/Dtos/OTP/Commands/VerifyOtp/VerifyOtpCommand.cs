using Alexapps.SkinCare.Enums;

namespace Alexapps.SkinCare.Business.OTP.Commands.VerifyOtp;

public class VerifyOtpCommand
{
    public string Username { get; init; }
    public string Otp { get; init; }
}


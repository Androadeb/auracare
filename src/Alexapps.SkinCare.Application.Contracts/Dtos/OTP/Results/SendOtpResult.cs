using System;

namespace Alexapps.SkinCare.Business.OTP.Results;

public class SendOtpResult
{
    public DateTime ExpiryTimeByMinute { get; init; }
    public string OtpCode { get; init; }
    public string PhoneNumber { get; init; }
    public string Email { get; init; }
}


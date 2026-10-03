using Alexapps.SkinCare.Entities.Base;
using Alexapps.SkinCare.Enums;
using System;

namespace Alexapps.SkinCare.Entities.Users;

public class Otp : BaseEntity
{
    public string PhoneCode { get; set; }
    public DateTime PhoneCodeExpireAt { get; set; }
    public string NewEmail { get; set; }
    public string NewPhone { get; set; }
    public string NewPhoneCode { get; set; }
    public DateTime? NewPhoneCodeExpireAt { get; set; }

    public int RetryCount { get; set; }
    public OtpTypeEnum Type { get; set; }

    public Guid UserId { get; set; }
    public User User { get; set; }
}


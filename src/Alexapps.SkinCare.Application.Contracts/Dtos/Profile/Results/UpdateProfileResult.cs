namespace Alexapps.SkinCare.Business.Client.Profile.Results;

public class UpdateProfileResult
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string PhoneCode { get; set; }
    public string PhoneNumber { get; set; }
    public int? OtpTtlInMinutes { get; set; } = null;
    public bool RequiresVerification { get; set; } = false;
}
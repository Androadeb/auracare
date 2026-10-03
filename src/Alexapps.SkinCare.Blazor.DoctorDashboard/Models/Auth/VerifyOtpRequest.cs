using System.ComponentModel.DataAnnotations;

namespace ynex.Models.Auth
{
    public class VerifyOtpRequest
    {
        [Required(ErrorMessage = "اسم المستخدم مطلوب")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "كود التحقق مطلوب")]
        // If your API expects 4 digits, change 6 to 4 below
        [StringLength(4, MinimumLength = 4, ErrorMessage = "كود التحقق يجب أن يتكون من 4 أرقام")]
        public string Otp { get; set; } = string.Empty;
    }
}

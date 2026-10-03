using System.ComponentModel.DataAnnotations;

namespace ynex.Models.Auth
{
    
    public class LoginRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
    }

   
    public class EmailLoginRequest
    {
        [Required(ErrorMessage = "EmailRequired")]
        [EmailAddress(ErrorMessage = "InvalidEmailFormat")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "PasswordRequired")]
        public string Password { get; set; } = string.Empty;
    }

    
    public class PhoneLoginRequest
    {
        [Required(ErrorMessage = "PhoneRequired")]
        [RegularExpression(@"^\+?[0-9]{7,15}$", ErrorMessage = "InvalidPhoneFormat")]
        public string Phone { get; set; } = string.Empty;
    }
}
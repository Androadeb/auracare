using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alexapps.SkinCare.Dtos.Profile.Commands.SendOtp
{
    public class SendOtpInput
    {
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
    }
}

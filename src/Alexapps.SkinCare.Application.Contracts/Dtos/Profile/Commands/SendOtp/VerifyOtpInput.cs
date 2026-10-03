using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alexapps.SkinCare.Dtos.Profile.Commands.SendOtp
{
    public class VerifyOtpInput : SendOtpInput
    {
        public string Code { get; set; }
    }
}

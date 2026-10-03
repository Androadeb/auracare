using Alexapps.SkinCare.Business.OTP.Commands.SendOtp;
using FluentValidation;

namespace Alexapps.SkinCare.Business.OTP.Validators;

public class SendOtpRequestValidator : AbstractValidator<SendOtpCommand>
{
    public SendOtpRequestValidator()
    {
        RuleFor(x => x.Username)
             .NotEmpty().WithMessage("Username is required.");

        RuleFor(x => x.Role)
            .NotEmpty().WithMessage("Role is required.");
    }
}


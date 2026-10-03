using Alexapps.SkinCare.Business.OTP.Commands.VerifyOtp;
using FluentValidation;

namespace Alexapps.SkinCare.Business.OTP.Validators;

public class VerifyOtpRequestValidator : AbstractValidator<VerifyOtpCommand>
{
    public VerifyOtpRequestValidator()
    {


        RuleFor(x => x.Username)
                .NotEmpty().WithMessage("Username is required.");

        RuleFor(x => x.Otp).NotEmpty().Matches(@"^\d{4}$");


    }
}


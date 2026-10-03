using Alexapps.SkinCare.Dtos.Auth.Commands.ForgetPassword;
using FluentValidation;

namespace Alexapps.SkinCare.Dtos.Auth.Validators.ForgetPassword;

public class ResendForgetPasswordOtpCommandValidator : AbstractValidator<ResendForgetPasswordOtpCommand>
{
    public ResendForgetPasswordOtpCommandValidator()
    {
        RuleFor(c => c.PhoneNumber)
             .NotEmpty()
             .NotNull()
             .MinimumLength(3)
             .MaximumLength(20);
    }
}



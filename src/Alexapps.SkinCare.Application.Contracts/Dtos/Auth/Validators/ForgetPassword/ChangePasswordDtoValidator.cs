using Alexapps.SkinCare.Dtos.Auth.Commands.ForgetPassword;
using FluentValidation;

namespace Alexapps.SkinCare.Dtos.Auth.Validators.ForgetPassword;

public class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordCommandValidator()
    {
        RuleFor(c => c.NewPassword)
            .NotEmpty()
            .NotNull()
            .Matches(@"^(?=.*[a-z])(?=.*\d).+$")
            .WithMessage("Password must contain at least one lowercase letter, and one digit.");

    }
}



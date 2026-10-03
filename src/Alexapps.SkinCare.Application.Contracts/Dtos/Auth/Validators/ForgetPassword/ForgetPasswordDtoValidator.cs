using Alexapps.SkinCare.Dtos.Auth.Commands.ForgetPassword;
using Alexapps.SkinCare.Enums;
using FluentValidation;

namespace Alexapps.SkinCare.Dtos.Auth.Validators.ForgetPassword;

public class ForgetPasswordCommandValidator : AbstractValidator<ForgetPasswordCommand>
{
    public ForgetPasswordCommandValidator()
    {
        RuleFor(c => c.PhoneNumber)
            .NotEmpty()
            .NotNull()
            .MinimumLength(3)
            .MaximumLength(20);

        // Rule for Role: Must not be empty or null and must be a valid role (user or admin)
        RuleFor(x => x.Role)
            .NotEmpty().WithMessage("Role is required.")
            .NotNull().WithMessage("Role cannot be null.")
            .Must(x => x == RoleEnum.CLIENT || x == RoleEnum.ADMIN || x == RoleEnum.DOCTOR || x == RoleEnum.LAB).WithMessage("Role is not valid.");
    }
}



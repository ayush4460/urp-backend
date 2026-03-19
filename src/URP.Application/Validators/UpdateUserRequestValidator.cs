using FluentValidation;
using URP.Application.DTOs.Users;

namespace URP.Application.Validators;

public sealed class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
{
    public UpdateUserRequestValidator()
    {
        When(x => !string.IsNullOrEmpty(x.NewPassword), () =>
        {
            RuleFor(x => x.CurrentPassword)
                .NotEmpty().WithMessage("Current password is required to change password.");

            RuleFor(x => x.NewPassword)
                .MinimumLength(8)
                .Matches("[A-Z]").WithMessage("New password must contain uppercase.")
                .Matches("[a-z]").WithMessage("New password must contain lowercase.")
                .Matches("[0-9]").WithMessage("New password must contain a digit.")
                .Matches("[^a-zA-Z0-9]").WithMessage("New password must contain a special character.");

            RuleFor(x => x.ConfirmNewPassword)
                .Equal(x => x.NewPassword).WithMessage("New passwords do not match.");
        });

        When(x => x.Username != null, () =>
            RuleFor(x => x.Username).MinimumLength(3).MaximumLength(50)
                .Matches(@"^[a-zA-Z0-9_.\-]+$").WithMessage("Username: invalid characters."));
    }
}

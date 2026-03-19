using FluentValidation;
using URP.Application.DTOs.Users;

namespace URP.Application.Validators;

public sealed class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
{
    public CreateUserRequestValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty()
            .MinimumLength(3).WithMessage("Username must be at least 3 characters.")
            .MaximumLength(50)
            .Matches(@"^[a-zA-Z0-9_.\-]+$").WithMessage("Username: letters, numbers, _ . - only.");

        RuleFor(x => x.Email)
            .NotEmpty().EmailAddress().MaximumLength(255);

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8).WithMessage("Password must be at least 8 characters.")
            .Matches("[A-Z]").WithMessage("Password must contain an uppercase letter.")
            .Matches("[a-z]").WithMessage("Password must contain a lowercase letter.")
            .Matches("[0-9]").WithMessage("Password must contain a digit.")
            .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain a special character.");

        RuleFor(x => x.ConfirmPassword)
            .Equal(x => x.Password).WithMessage("Passwords do not match.");

        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
    }
}

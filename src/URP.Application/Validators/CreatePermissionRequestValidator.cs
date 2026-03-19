using FluentValidation;
using URP.Application.DTOs.Permissions;

namespace URP.Application.Validators;

public sealed class CreatePermissionRequestValidator : AbstractValidator<CreatePermissionRequest>
{
    public CreatePermissionRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().MaximumLength(100)
            .Matches(@"^[a-z]+:[a-z_]+$")
            .WithMessage("Format must be resource:action (e.g. users:read).");

        RuleFor(x => x.Group).NotEmpty().MaximumLength(50);
    }
}

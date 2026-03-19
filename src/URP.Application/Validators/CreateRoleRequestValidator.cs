using FluentValidation;
using URP.Application.DTOs.Roles;

namespace URP.Application.Validators;

public sealed class CreateRoleRequestValidator : AbstractValidator<CreateRoleRequest>
{
    public CreateRoleRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MinimumLength(2).MaximumLength(50);
        RuleFor(x => x.Description).MaximumLength(255).When(x => x.Description != null);
    }
}

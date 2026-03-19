namespace URP.Application.DTOs.Roles;

public sealed class CreateRoleRequest
{
    public string  Name        { get; init; } = default!;
    public string? Description { get; init; }
}

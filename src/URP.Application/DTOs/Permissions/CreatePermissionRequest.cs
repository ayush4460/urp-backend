namespace URP.Application.DTOs.Permissions;

public sealed class CreatePermissionRequest
{
    public string  Name        { get; init; } = default!;
    public string? Description { get; init; }
    public string  Group       { get; init; } = default!;
}

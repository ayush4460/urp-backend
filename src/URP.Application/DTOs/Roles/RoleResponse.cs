using URP.Application.DTOs.Permissions;

namespace URP.Application.DTOs.Roles;

public sealed class RoleResponse
{
    public int    Id          { get; init; }
    public string Name        { get; init; } = default!;
    public string? Description { get; init; }
    public bool   IsActive    { get; init; }
    /// <summary>Unix epoch seconds (UTC).</summary>
    public long   CreatedAt   { get; init; }
    public List<PermissionResponse> Permissions { get; init; } = new();
}

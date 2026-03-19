namespace URP.Application.DTOs.Permissions;

public sealed class AssignPermissionRequest
{
    public int RoleId       { get; init; }
    public int PermissionId { get; init; }
}

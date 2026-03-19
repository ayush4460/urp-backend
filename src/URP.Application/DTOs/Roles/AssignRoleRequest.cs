namespace URP.Application.DTOs.Roles;

public sealed class AssignRoleRequest
{
    public long UserId { get; init; }
    public int  RoleId { get; init; }
}

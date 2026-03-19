namespace URP.Application.DTOs.Roles;

public sealed class RemoveRoleRequest
{
    public long UserId { get; init; }
    public int  RoleId { get; init; }
}

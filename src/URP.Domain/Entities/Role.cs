using URP.Domain.Common;

namespace URP.Domain.Entities;

public sealed class Role : BaseEntity<int>
{
    private Role() { }

    public string Name { get; private set; } = default!;
    public string? Description { get; private set; }
    public bool IsActive { get; private set; } = true;

    public ICollection<UserRole> UserRoles { get; private set; } = new List<UserRole>();
    public ICollection<RolePermission> RolePermissions { get; private set; } = new List<RolePermission>();

    public static Role Create(string name, string? description = null) =>
        new() { Name = name.Trim(), Description = description?.Trim(), IsActive = true };

    public void Update(string name, string? description)
    {
        Name = name.Trim();
        Description = description?.Trim();
        Touch();
    }
}   
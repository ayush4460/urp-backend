using URP.Domain.Common;

namespace URP.Domain.Entities;

public sealed class Permission : BaseEntity<int>
{
    private Permission() { }

    public string Name { get; private set; } = default!;
    public string? Description { get; private set; }
    public string Group { get; private set; } = default!;

    public ICollection<RolePermission> RolePermissions { get; private set; } = new List<RolePermission>();

    public static Permission Create(string name, string group, string? description = null) =>
        new()
        {
            Name = name.Trim().ToLowerInvariant(),
            Group = group.Trim(),
            Description = description?.Trim(),
        };
}
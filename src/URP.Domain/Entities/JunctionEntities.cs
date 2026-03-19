using URP.Domain.Common;

namespace URP.Domain.Entities;

public sealed class UserRole
{
    private UserRole() { }

    public long UserId { get; private set; }
    public int RoleId { get; private set; }
    public long AssignedAt { get; private set; } = EpochHelper.NowSeconds();
    public long? AssignedBy { get; private set; }

    public User User { get; private set; } = default!;
    public Role Role { get; private set; } = default!;

    public static UserRole Create(long userId, int roleId, long? assignedBy = null) =>
        new() { UserId = userId, RoleId = roleId, AssignedBy = assignedBy };
}

public sealed class RolePermission
{
    private RolePermission() { }

    public int RoleId { get; private set; }
    public int PermissionId { get; private set; }
    public long AssignedAt { get; private set; } = EpochHelper.NowSeconds();

    public Role Role { get; private set; } = default!;
    public Permission Permission { get; private set; } = default!;

    public static RolePermission Create(int roleId, int permissionId) =>
        new() { RoleId = roleId, PermissionId = permissionId };
}

public sealed class RefreshToken : BaseEntity<long>
{
    private RefreshToken() { }

    public long UserId { get; private set; }
    public string Token { get; private set; } = default!;
    public long ExpiresAt { get; private set; }
    public long? RevokedAt { get; private set; }
    public string? ReplacedBy { get; private set; }

    public bool IsActive => RevokedAt == null && EpochHelper.NowSeconds() < ExpiresAt;
    public bool IsExpired => EpochHelper.NowSeconds() >= ExpiresAt;

    public User User { get; private set; } = default!;

    public static RefreshToken Create(long userId, string token, int expiryDays) =>
        new()
        {
            UserId = userId,
            Token = token,
            ExpiresAt = EpochHelper.NowSeconds() + (expiryDays * 86400L),
        };

    public void Revoke(string? replacedBy = null)
    {
        RevokedAt = EpochHelper.NowSeconds();
        ReplacedBy = replacedBy;
    }
}
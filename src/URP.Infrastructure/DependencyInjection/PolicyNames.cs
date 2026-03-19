namespace URP.Infrastructure.DependencyInjection;

public static class PolicyNames
{
    public const string UsersRead        = "users:read";
    public const string UsersCreate      = "users:create";
    public const string UsersUpdate      = "users:update";
    public const string UsersDelete      = "users:delete";
    public const string RolesRead        = "roles:read";
    public const string RolesCreate      = "roles:create";
    public const string RolesUpdate      = "roles:update";
    public const string RolesDelete      = "roles:delete";
    public const string RolesAssign      = "roles:assign";
    public const string PermissionsRead   = "permissions:read";
    public const string PermissionsCreate = "permissions:create";
    public const string PermissionsAssign = "permissions:assign";

    public static readonly IReadOnlyList<string> All = new[]
    {
        UsersRead, UsersCreate, UsersUpdate, UsersDelete,
        RolesRead, RolesCreate, RolesUpdate, RolesDelete, RolesAssign,
        PermissionsRead, PermissionsCreate, PermissionsAssign,
    };
}

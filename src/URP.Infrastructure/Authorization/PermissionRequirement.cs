using Microsoft.AspNetCore.Authorization;

namespace URP.Infrastructure.Authorization;

public sealed class PermissionRequirement(string permission) : IAuthorizationRequirement
{
    public string Permission { get; } = permission;
}

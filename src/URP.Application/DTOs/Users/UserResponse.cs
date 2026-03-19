using URP.Application.DTOs.Roles;

namespace URP.Application.DTOs.Users;

public sealed class UserResponse
{
    public long   Id          { get; init; }
    public string Username    { get; init; } = default!;
    public string Email       { get; init; } = default!;
    public string FirstName   { get; init; } = default!;
    public string LastName    { get; init; } = default!;
    public string FullName    { get; init; } = default!;
    public bool   IsActive    { get; init; }
    /// <summary>Unix epoch seconds (UTC). Null = never logged in.</summary>
    public long?  LastLoginAt { get; init; }
    /// <summary>Unix epoch seconds (UTC). Frontend converts to IST.</summary>
    public long   CreatedAt   { get; init; }
    /// <summary>Unix epoch seconds (UTC). Frontend converts to IST.</summary>
    public long   UpdatedAt   { get; init; }
    public List<RoleResponse> Roles       { get; init; } = new();
    public List<string>       Permissions { get; init; } = new();
}

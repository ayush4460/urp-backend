namespace URP.Application.DTOs.Permissions;

public sealed class PermissionResponse
{
    public int    Id          { get; init; }
    public string Name        { get; init; } = default!;
    public string? Description { get; init; }
    public string Group       { get; init; } = default!;
    /// <summary>Unix epoch seconds (UTC).</summary>
    public long   CreatedAt   { get; init; }
}

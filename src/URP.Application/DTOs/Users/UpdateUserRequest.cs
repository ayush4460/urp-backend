namespace URP.Application.DTOs.Users;

public sealed class UpdateUserRequest
{
    public string? FirstName          { get; init; }
    public string? LastName           { get; init; }
    public string? Username           { get; init; }
    public bool?   IsActive           { get; init; }
    public string? CurrentPassword    { get; init; }
    public string? NewPassword        { get; init; }
    public string? ConfirmNewPassword { get; init; }
}

namespace URP.Application.DTOs.Users;

public sealed class CreateUserRequest
{
    public string Username        { get; init; } = default!;
    public string Email           { get; init; } = default!;
    public string Password        { get; init; } = default!;
    public string ConfirmPassword { get; init; } = default!;
    public string FirstName       { get; init; } = default!;
    public string LastName        { get; init; } = default!;
}

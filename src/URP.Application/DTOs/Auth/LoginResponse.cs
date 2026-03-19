using URP.Application.DTOs.Users;

namespace URP.Application.DTOs.Auth;

public sealed class LoginResponse
{
    public string       AccessToken  { get; init; } = default!;
    public string       TokenType    { get; init; } = "Bearer";
    public int          ExpiresIn    { get; init; }
    public string       RefreshToken { get; init; } = default!;
    public UserResponse User         { get; init; } = default!;
}

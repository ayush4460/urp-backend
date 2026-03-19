using URP.Domain.Entities;

namespace URP.Application.Interfaces;

public record TokenResult(string AccessToken, string RefreshToken, int ExpiresInSeconds);

public interface ITokenService
{
    TokenResult GenerateToken(User user);
}

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using URP.Application.Interfaces;
using URP.Domain.Entities;
using URP.Infrastructure.DependencyInjection;

namespace URP.Infrastructure.Services;

public sealed class JwtTokenService(IOptions<JwtSettings> options) : ITokenService
{
    private readonly JwtSettings _cfg = options.Value;

    public TokenResult GenerateToken(User user)
    {
        var roles = user.UserRoles.Select(ur => ur.Role.Name).ToList();
        var permissions = user.UserRoles
            .SelectMany(ur => ur.Role.RolePermissions)
            .Select(rp => rp.Permission.Name)
            .Distinct().ToList();

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub,   user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(JwtRegisteredClaimNames.Jti,   Guid.NewGuid().ToString()),
            new(ClaimTypes.Name,               user.Username),
            new(AppClaimTypes.UserId,          user.Id.ToString()),
            new(AppClaimTypes.FullName,        user.FullName),
        };

        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));
        claims.AddRange(permissions.Select(p => new Claim(AppClaimTypes.Permission, p)));

        var key    = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_cfg.SecretKey));
        var creds  = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiry = _cfg.AccessTokenExpirationMinutes;

        var token = new JwtSecurityToken(
            issuer:   _cfg.Issuer, audience: _cfg.Audience,
            claims:   claims,
            notBefore: DateTime.UtcNow,
            expires:   DateTime.UtcNow.AddMinutes(expiry),
            signingCredentials: creds);

        return new TokenResult(
            new JwtSecurityTokenHandler().WriteToken(token),
            GenerateRefreshToken(),
            expiry * 60);
    }

    private static string GenerateRefreshToken()
    {
        var bytes = new byte[64];
        RandomNumberGenerator.Fill(bytes);
        return Convert.ToBase64String(bytes);
    }
}

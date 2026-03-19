using System.Security.Claims;
using URP.Infrastructure.DependencyInjection;

namespace URP.API.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static long GetUserId(this ClaimsPrincipal principal)
    {
        var claim = principal.FindFirst(AppClaimTypes.UserId)
            ?? throw new UnauthorizedAccessException("User ID claim not found.");
        return long.Parse(claim.Value);
    }

    public static bool HasPermission(this ClaimsPrincipal principal, string permission)
        => principal.Claims.Any(c => c.Type == AppClaimTypes.Permission && c.Value == permission);
}

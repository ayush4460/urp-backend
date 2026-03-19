using Microsoft.AspNetCore.Authorization;
using URP.Infrastructure.DependencyInjection;

namespace URP.Infrastructure.Authorization;

public sealed class PermissionAuthorizationHandler
    : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext ctx, PermissionRequirement req)
    {
        var has = ctx.User.Claims
            .Where(c => c.Type == AppClaimTypes.Permission)
            .Any(c => c.Value == req.Permission);

        if (has) ctx.Succeed(req);
        return Task.CompletedTask;
    }
}

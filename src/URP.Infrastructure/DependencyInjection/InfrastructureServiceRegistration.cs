using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using URP.Application.Interfaces;
using URP.Domain.Repositories;
using URP.Infrastructure.Authorization;
using URP.Infrastructure.Persistence;
using URP.Infrastructure.Repositories;
using URP.Infrastructure.Services;

namespace URP.Infrastructure.DependencyInjection;

public static class InfrastructureServiceRegistration
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services, IConfiguration config)
    {
        // ── Database ──────────────────────────────────────────────────────────
        var conn = config.GetConnectionString("DefaultConnection")!;
        services.AddDbContext<ApplicationDbContext>(o =>
            o.UseMySql(conn, ServerVersion.AutoDetect(conn),
                m => m.EnableRetryOnFailure(3).CommandTimeout(30)));

        // ── Repositories ──────────────────────────────────────────────────────
        services.AddScoped<IUserRepository,       UserRepository>();
        services.AddScoped<IRoleRepository,       RoleRepository>();
        services.AddScoped<IPermissionRepository, PermissionRepository>();
        services.AddScoped<IUnitOfWork,           UnitOfWork>();

        // ── Infrastructure Services ───────────────────────────────────────────
        services.AddScoped<ITokenService,   JwtTokenService>();
        services.AddScoped<IPasswordService, PasswordService>();

        // ── JWT Authentication ────────────────────────────────────────────────
        var jwtSection = config.GetSection(JwtSettings.SectionName);
        services.Configure<JwtSettings>(jwtSection);
        var jwt = jwtSection.Get<JwtSettings>()!;

        services.AddAuthentication(o =>
        {
            o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            o.DefaultChallengeScheme    = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(o =>
        {
            o.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey         = new SymmetricSecurityKey(
                                               Encoding.UTF8.GetBytes(jwt.SecretKey)),
                ValidateIssuer   = true, ValidIssuer   = jwt.Issuer,
                ValidateAudience = true, ValidAudience = jwt.Audience,
                ValidateLifetime = true,
                ClockSkew        = TimeSpan.Zero,
            };
            o.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = ctx =>
                {
                    if (ctx.Exception is SecurityTokenExpiredException)
                        ctx.Response.Headers["Token-Expired"] = "true";
                    return Task.CompletedTask;
                },
                OnChallenge = ctx =>
                {
                    ctx.HandleResponse();
                    return Task.CompletedTask;
                },
            };
        });

        // ── Permission Policies ───────────────────────────────────────────────
        services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();
        services.AddAuthorization(o =>
        {
            foreach (var policy in PolicyNames.All)
                o.AddPolicy(policy, b =>
                    b.RequireAuthenticatedUser()
                     .AddRequirements(new PermissionRequirement(policy)));
        });

        return services;
    }
}

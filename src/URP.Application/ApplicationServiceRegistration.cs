using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using URP.Application.Interfaces;
using URP.Application.Services;

namespace URP.Application;

public static class ApplicationServiceRegistration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(ApplicationServiceRegistration).Assembly);
        services.AddValidatorsFromAssembly(typeof(ApplicationServiceRegistration).Assembly);
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IPermissionService, PermissionService>();
        return services;
    }
}
using Microsoft.OpenApi.Models;

namespace URP.API.Extensions;

public static class SwaggerExtensions
{
    public static IServiceCollection AddSwaggerWithJwt(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "URP — User Role Permission API",
                Version = "v1",
                Description = """
                    **Clean Architecture** · Domain → Application → Infrastructure → API

                    ### Quick Start
                    1. `POST /api/v1/users/login` with `superadmin@urp.local` / `Admin@123`
                    2. Copy the `accessToken` from the response
                    3. Click **Authorize 🔓** above → paste the token → **Authorize** → **Close**
                    4. All 🔒 endpoints are now accessible

                    All timestamps are **Unix epoch seconds (UTC)** — frontend converts to IST.
                    """
            });

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization", Type = SecuritySchemeType.Http,
                Scheme = "Bearer", BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Paste your JWT token here (without the 'Bearer ' prefix)."
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                    },
                    Array.Empty<string>()
                }
            });

            var xml = Path.Combine(AppContext.BaseDirectory, "URP.API.xml");
            if (File.Exists(xml)) c.IncludeXmlComments(xml);
        });

        return services;
    }
}

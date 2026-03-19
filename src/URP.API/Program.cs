using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Serilog;
using URP.API.Extensions;
using URP.API.Middleware;
using URP.Application;
using URP.Application.Interfaces;
using URP.Infrastructure.DependencyInjection;
using URP.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, cfg) => cfg.ReadFrom.Configuration(ctx.Configuration));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddSwaggerWithJwt();

// Clean Architecture layer registration
builder.Services.AddApplicationServices();    
builder.Services.AddInfrastructureServices(builder.Configuration); 

builder.Services.AddCors(o =>
    o.AddPolicy("AllowReact", p =>
        p.WithOrigins(
            builder.Configuration["Cors:AllowedOrigins"]
                ?.Split(',', StringSplitOptions.RemoveEmptyEntries)
            ?? ["http://localhost:5173"])
         .AllowAnyMethod().AllowAnyHeader().AllowCredentials()));

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "URP API v1");
    c.RoutePrefix = "swagger";
    c.DocumentTitle = "URP API — Swagger UI";
    c.DefaultModelsExpandDepth(-1);
    c.DisplayRequestDuration();
});

app.UseHttpsRedirection();
app.UseCors("AllowReact");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Migrate + seed on startup
using (var scope = app.Services.CreateScope())
{
    var db      = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var pwdSvc  = scope.ServiceProvider.GetRequiredService<IPasswordService>();
    var log     = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    try
    {
        log.LogInformation("Applying migrations...");
        await db.Database.MigrateAsync();
        log.LogInformation("Seeding database...");
        await DataSeeder.SeedAsync(db, pwdSvc, log);
        log.LogInformation("Ready → http://localhost:5000/swagger | superadmin@urp.local / Admin@123");
    }
    catch (Exception ex)
    {
        log.LogError(ex, "Startup failed. Check MySQL connection in appsettings.json.");
    }
}

await app.RunAsync();

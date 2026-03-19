using System.Diagnostics;
using System.Net;
using System.Text.Json;
using URP.Application.Common;
using URP.Domain.Exceptions;

namespace URP.API.Middleware;

public sealed class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
{
    public async Task InvokeAsync(HttpContext ctx)
    {
        try { await next(ctx); }
        catch (Exception ex) { await HandleAsync(ctx, ex); }
    }

    private async Task HandleAsync(HttpContext ctx, Exception ex)
    {
        var (code, msg) = ex switch
        {
            NotFoundException     e => (HttpStatusCode.NotFound,           e.Message),
            ConflictException     e => (HttpStatusCode.Conflict,            e.Message),
            UnauthorizedException e => (HttpStatusCode.Unauthorized,        e.Message),
            ForbiddenException    e => (HttpStatusCode.Forbidden,           e.Message),
            BusinessRuleException e => (HttpStatusCode.BadRequest,          e.Message),
            FluentValidation.ValidationException v =>
                (HttpStatusCode.BadRequest, string.Join(" | ", v.Errors.Select(e => e.ErrorMessage))),
            _ => (HttpStatusCode.InternalServerError, "An unexpected error occurred.")
        };

        if (code == HttpStatusCode.InternalServerError)
            logger.LogError(ex, "Unhandled: {Method} {Path}", ctx.Request.Method, ctx.Request.Path);
        else
            logger.LogWarning("{Type}: {Msg} | {Method} {Path}",
                ex.GetType().Name, ex.Message, ctx.Request.Method, ctx.Request.Path);

        ctx.Response.ContentType = "application/json";
        ctx.Response.StatusCode  = (int)code;

        var resp = ApiResponse.Fail(msg, env.IsDevelopment() ? new List<string> { ex.ToString() } : null);
        await ctx.Response.WriteAsync(JsonSerializer.Serialize(resp,
            new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }));
    }
}

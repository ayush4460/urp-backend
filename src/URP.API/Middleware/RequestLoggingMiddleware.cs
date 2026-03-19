using System.Diagnostics;

namespace URP.API.Middleware;

public sealed class RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext ctx)
    {
        var sw = Stopwatch.StartNew();
        logger.LogDebug("→ {Method} {Path}", ctx.Request.Method, ctx.Request.Path);
        await next(ctx);
        sw.Stop();
        var level = ctx.Response.StatusCode >= 400
            ? LogLevel.Warning : LogLevel.Debug;
        logger.Log(level, "← {Method} {Path} | {Status} | {Ms}ms",
            ctx.Request.Method, ctx.Request.Path, ctx.Response.StatusCode, sw.ElapsedMilliseconds);
    }
}

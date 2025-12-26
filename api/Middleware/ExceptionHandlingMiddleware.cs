using System.Net;
using System.Text.Json;
using AgendaSync.Exceptions;

namespace AgendaSync.Middleware;

public class ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger) : IMiddleware
{
    private readonly ILogger<ExceptionHandlingMiddleware> _logger = logger;

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (GoogleRefreshTokenExpiredException ex)
        {
            _logger.LogWarning(ex, "Expired refresh token");

            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsJsonAsync(new
            {
                status = 401,
                title = "Refresh Token Expired",
                detail = "Your session has expired. Please log in again.",
                traceId = context.TraceIdentifier
            });

            return;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception: {Message}", ex.Message);

            var problem = new
            {
                status = (int)HttpStatusCode.InternalServerError,
                title = "Internal Server Error",
                detail = "An unexpected error occurred. Please try again later.",
                traceId = context.TraceIdentifier
            };

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(problem));
        }
    }
}

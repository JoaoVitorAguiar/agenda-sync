using AgendaSync.Dtos;
using AgendaSync.Services.Interfaces;

namespace AgendaSync.Routes;

public static class AuthRoutes
{
    public static void MapAuthRoutes(this IEndpointRouteBuilder app)
    {
        var authGroup = app.MapGroup("/auth")
            .WithTags("Authentication");

        authGroup.MapPost("/google/login", async (
            GoogleAuthRequest request,
            IAuthService authService,
            HttpContext ctx,
            IHostEnvironment env
        ) =>
        {
            var jwt = await authService.AuthenticateAsync(request.Code);

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = !env.IsDevelopment(),
                SameSite = env.IsDevelopment()
                    ? SameSiteMode.Lax
                    : SameSiteMode.Strict,
                Path = "/",
                Expires = DateTimeOffset.UtcNow.AddDays(1)
            };

            ctx.Response.Cookies.Append("agenda_token", jwt, cookieOptions);

            return Results.Ok(new { message = "Logged in successfully" });
        });


        authGroup.MapGet("/check", () =>
            Results.NoContent()
        )
        .RequireAuthorization();
    }
}


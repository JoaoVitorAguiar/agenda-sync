using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AgendaSync.Dtos;
using AgendaSync.Security;
using AgendaSync.Services;
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
            HttpContext ctx
        ) =>
        {
            var jwt = await authService.AuthenticateAsync(request.Code);

            ctx.Response.Cookies.Append(
                "agenda_token",
                jwt,
                new CookieOptions
                {
                    HttpOnly = true,
                    Secure = !ctx.Request.Host.Host.Contains("localhost"),
                    SameSite = SameSiteMode.Strict,
                    Path = "/",
                    Expires = DateTimeOffset.UtcNow.AddDays(7)
                }
            );

            return Results.Ok(new { message = "Logged in successfully" });
        });

        authGroup.MapGet("/check", () =>
            Results.NoContent()
        )
        .RequireAuthorization();
    }
}


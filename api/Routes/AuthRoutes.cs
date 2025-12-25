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
            IAuthService authService
        ) =>
        {
            var jwt = await authService.AuthenticateAsync(request.Code);

            return Results.Ok(new
            {
                accessToken = jwt
            });
        });
    }
}


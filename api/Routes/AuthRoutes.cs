
using AgendaSync.Services;

namespace AgendaSync.Routes;

public static class AuthRoutes
{
    public static void MapAuthRoutes(this IEndpointRouteBuilder app)
    {
        var authGroup = app.MapGroup("/auth")
            .WithTags("Authentication");

        authGroup.MapPost("/google", async (GoogleAuthRequest request, IAuthService authService) =>
        {
            var jwt = await authService.AuthenticateAsync(request.Code);

            return Results.Ok(new
            {
                accessToken = jwt
            });
        });
    }
}

public record GoogleAuthRequest(string Code);



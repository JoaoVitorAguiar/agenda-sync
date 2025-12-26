using AgendaSync.Dtos;
using AgendaSync.Security;
using AgendaSync.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AgendaSync.Routes;

public static class EventRoutes
{
    public static void MapEventRoutes(this IEndpointRouteBuilder app)
    {
        var eventGroup = app.MapGroup("/events")
            .WithTags("Events");

        eventGroup.MapGet("/", async (
            [FromServices] AuthUser user,
            [FromServices] IUserRepository userRepository,
            [FromServices] IAuthService authService,
            [FromServices] IEventService eventService) =>
        {
            var userEntity = await userRepository.GetByIdAsync(user.Id);

            if (userEntity?.ExternalRefreshToken is null)
                return Results.BadRequest("No Google refresh token");

            var accessToken =
                await authService.GetAccessTokenAsync(userEntity.ExternalRefreshToken);

            var events =
                await eventService.GetNextEventsAsync(accessToken, 7);

            return Results.Ok(events);
        })
        .RequireAuthorization()
        .Produces<List<EventDto>>(StatusCodes.Status200OK);
    }
}


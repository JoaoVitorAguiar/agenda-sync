using AgendaSync.Dtos;
using AgendaSync.Repositories.Interfaces;
using AgendaSync.Security;
using AgendaSync.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AgendaSync.Routes;

public static class EventRoutes
{
    public static void MapEventRoutes(this IEndpointRouteBuilder app)
    {
        var eventGroup = app.MapGroup("/events")
            .WithTags("Events")
            .RequireAuthorization();

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

            var events = await eventService.GetNextEventsAsync(accessToken, 7);

            return Results.Ok(events);
        })
        .Produces<List<EventDto>>(StatusCodes.Status200OK);


        eventGroup.MapGet("/{id}", async (
            string id,
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

            var evt = await eventService.GetEventByIdAsync(accessToken, id);

            return Results.Ok(evt);
        })
        .Produces<EventDetailsDto>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);


        eventGroup.MapPost("/", async (
            [FromBody] EventCreateDto dto,
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

            var eventId = await eventService.CreateEventAsync(accessToken, dto);

            return Results.Created($"/events/{eventId}", new { id = eventId });
        })
        .Produces(StatusCodes.Status201Created);
    }
}

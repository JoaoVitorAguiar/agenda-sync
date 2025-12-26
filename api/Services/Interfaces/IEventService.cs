using AgendaSync.Dtos;

namespace AgendaSync.Services.Interfaces;

public interface IEventService
{
    Task<List<EventDto>> GetNextEventsAsync(string accessToken, int days = 7);
    Task<EventDetailsDto?> GetEventByIdAsync(string accessToken, string eventId);
    Task<string> CreateEventAsync(string accessToken, EventCreateDto dto);
}

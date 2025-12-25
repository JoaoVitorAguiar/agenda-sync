using AgendaSync.Dtos;

namespace AgendaSync.Services.Interfaces;

public interface IEventService
{
    Task<List<EventDto>> GetNextEventsAsync(string accessToken, int days = 7);
}


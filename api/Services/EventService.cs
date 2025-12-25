using System.Net.Http.Headers;
using System.Text.Json;
using AgendaSync.Dtos;
using AgendaSync.Services.Interfaces;

namespace AgendaSync.Services;

public class EventService(HttpClient http) : IEventService
{
    private readonly HttpClient _http = http;

    public async Task<List<EventDto>> GetNextEventsAsync(string accessToken, int days = 7)
    {
        var now = DateTime.UtcNow;
        var until = now.AddDays(days);

        var request = new HttpRequestMessage(
            HttpMethod.Get,
            $"https://www.googleapis.com/calendar/v3/calendars/primary/events" +
            $"?timeMin={now:o}&timeMax={until:o}&singleEvents=true&orderBy=startTime"
        );

        request.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await _http.SendAsync(request);

        var body = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            throw new Exception($"Google Calendar error: {body}");

        var json = JsonSerializer.Deserialize<JsonElement>(body);

        var events = new List<EventDto>();

        foreach (var item in json.GetProperty("items").EnumerateArray())
        {
            events.Add(new EventDto(
                Id: item.GetProperty("id").GetString()!,
                Summary: item.GetProperty("summary").GetString(),
                Start: item.GetProperty("start").TryGetProperty("dateTime", out var s)
                    ? s.GetDateTime()
                    : item.GetProperty("start").GetProperty("date").GetDateTime(),
                End: item.GetProperty("end").TryGetProperty("dateTime", out var e)
                    ? e.GetDateTime()
                    : item.GetProperty("end").GetProperty("date").GetDateTime()
            ));
        }

        return events;
    }
}
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using AgendaSync.Dtos;
using AgendaSync.Services.Interfaces;

namespace AgendaSync.Services;

public class EventService(IHttpClientFactory httpClientFactory) : IEventService
{
    private readonly HttpClient _http = httpClientFactory.CreateClient("GoogleCalendar");

    public async Task<List<EventDto>> GetNextEventsAsync(string accessToken, int days = 7)
    {
        var now = DateTime.UtcNow;
        var until = now.AddDays(days);

        var url = $"primary/events?timeMin={now:o}&timeMax={until:o}&singleEvents=true&orderBy=startTime";

        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await _http.SendAsync(request);

        var body = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            throw new Exception($"Google Calendar error: {body}");

        var json = JsonSerializer.Deserialize<JsonElement>(body);

        var events = new List<EventDto>();

        foreach (var item in json.GetProperty("items").EnumerateArray())
        {
            var summary = item.TryGetProperty("summary", out var sm)
                ? sm.GetString()
                : "(Sem título)";

            DateTime start = DateTime.MinValue;
            DateTime end = DateTime.MinValue;

            if (item.TryGetProperty("start", out var st))
            {
                if (st.TryGetProperty("dateTime", out var sdt))
                    start = sdt.GetDateTime();
                else if (st.TryGetProperty("date", out var sd))
                    start = sd.GetDateTime();
            }

            if (item.TryGetProperty("end", out var et))
            {
                if (et.TryGetProperty("dateTime", out var edt))
                    end = edt.GetDateTime();
                else if (et.TryGetProperty("date", out var ed))
                    end = ed.GetDateTime();
            }

            events.Add(new EventDto(
                Id: item.GetProperty("id").GetString()!,
                Summary: summary,
                Start: start,
                End: end
            ));
        }

        return events;
    }

    public async Task<EventDetailsDto?> GetEventByIdAsync(string accessToken, string eventId)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"primary/events/{eventId}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await _http.SendAsync(request);
        var body = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            throw new Exception($"Google API Error: {body}");

        var json = JsonDocument.Parse(body).RootElement;


        string summary = json.TryGetProperty("summary", out var sm)
            ? sm.GetString() ?? "(Sem título)"
            : "(Sem título)";

        DateTime start = DateTime.MinValue;
        if (json.TryGetProperty("start", out var st))
        {
            if (st.TryGetProperty("dateTime", out var sdt))
                start = sdt.GetDateTime();
            else if (st.TryGetProperty("date", out var sd))
                start = sd.GetDateTime();
        }

        DateTime end = DateTime.MinValue;
        if (json.TryGetProperty("end", out var et))
        {
            if (et.TryGetProperty("dateTime", out var edt))
                end = edt.GetDateTime();
            else if (et.TryGetProperty("date", out var ed))
                end = ed.GetDateTime();
        }

        string? description = json.TryGetProperty("description", out var desc) ? desc.GetString() : null;
        string? location = json.TryGetProperty("location", out var loc) ? loc.GetString() : null;
        string? htmlLink = json.TryGetProperty("htmlLink", out var link) ? link.GetString() : null;

        string? creatorEmail =
            json.TryGetProperty("creator", out var creator) &&
            creator.TryGetProperty("email", out var ce)
                ? ce.GetString()
                : null;

        string? organizerEmail =
            json.TryGetProperty("organizer", out var org) &&
            org.TryGetProperty("email", out var oe)
                ? oe.GetString()
                : null;

        string? status = json.TryGetProperty("status", out var stt) ? stt.GetString() : null;
        string? eventType = json.TryGetProperty("eventType", out var etp) ? etp.GetString() : null;

        return new EventDetailsDto(
            Id: eventId,
            Summary: summary,
            Start: start,
            End: end,
            Description: description,
            Location: location,
            HtmlLink: htmlLink,
            CreatorEmail: creatorEmail,
            OrganizerEmail: organizerEmail,
            Status: status,
            EventType: eventType
        );
    }


    public async Task<string> CreateEventAsync(string accessToken, EventCreateDto dto)
    {
        var payload = new
        {
            summary = dto.Summary,
            description = dto.Description,
            location = dto.Location,
            start = new
            {
                dateTime = dto.Start.ToString("yyyy-MM-ddTHH:mm:sszzz"),
                timeZone = dto.TimeZone
            },
            end = new
            {
                dateTime = dto.End.ToString("yyyy-MM-ddTHH:mm:sszzz"),
                timeZone = dto.TimeZone
            }
        };

        var request = new HttpRequestMessage(HttpMethod.Post, "primary/events")
        {
            Headers =
            {
                Authorization = new AuthenticationHeaderValue("Bearer", accessToken)
            },
            Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json")
        };

        request.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", accessToken);

        request.Content = new StringContent(
            JsonSerializer.Serialize(payload),
            Encoding.UTF8,
            "application/json"
        );

        var response = await _http.SendAsync(request);
        var body = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            throw new Exception($"Google API Error: {body}");

        var json = JsonDocument.Parse(body).RootElement;

        return json.GetProperty("id").GetString()!;
    }
}
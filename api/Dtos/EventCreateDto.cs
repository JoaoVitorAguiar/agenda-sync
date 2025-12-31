namespace AgendaSync.Dtos;

public record EventCreateDto(
    string Summary,
    string Start,
    string End,
    string TimeZone,
    string? Description = null,
    string? Location = null
);

namespace AgendaSync.Dtos;

public record EventCreateDto(
    string Summary,
    DateTime Start,
    DateTime End,
    string? Description = null,
    string? Location = null,
    string? TimeZone = "UTC"
);

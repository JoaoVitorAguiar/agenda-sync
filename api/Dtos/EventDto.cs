
namespace AgendaSync.Dtos;

public record EventDto(
    string Id,
    string? Summary,
    DateTime Start,
    DateTime End
);
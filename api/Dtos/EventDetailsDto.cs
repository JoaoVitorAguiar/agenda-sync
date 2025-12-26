namespace AgendaSync.Dtos;

public record EventDetailsDto(
    string Id,
    string? Summary,
    DateTime Start,
    DateTime End,
    string? Description,
    string? Location,
    string? HtmlLink,
    string? CreatorEmail,
    string? OrganizerEmail,
    string? Status,
    string? EventType
);

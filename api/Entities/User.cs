namespace AgendaSync.Entities;

public class User(string email, string name, string externalSubject, string externalRefreshToken)
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Email { get; private set; } = email;
    public string Name { get; private set; } = name;
    public string ExternalSubject { get; private set; } = externalSubject;
    public string? ExternalRefreshToken { get; set; } = externalRefreshToken;
}

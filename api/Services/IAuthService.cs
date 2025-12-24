namespace AgendaSync.Services;

public interface IAuthService
{
    Task<string> AuthenticateAsync(string authorizationCode);
}

using AgendaSync.Dtos;

namespace AgendaSync.Services.Interfaces;

public interface IAuthService
{
    Task<string> AuthenticateAsync(string authorizationCode);
    Task<GoogleTokenResponse> ExchangeCodeForTokensAsync(string code);
    Task<string> GetAccessTokenAsync(string refreshToken);
}


using System.Text.Json;
using AgendaSync.Dtos;
using AgendaSync.Entities;
using AgendaSync.Exceptions;
using AgendaSync.Repositories.Interfaces;
using AgendaSync.Services.Interfaces;
using Google.Apis.Auth;

namespace AgendaSync.Services;

public class GoogleAuthService(IConfiguration config, IHttpClientFactory httpClientFactory, IJwtProvider jwtProvider, IUserRepository userRepository) : IAuthService
{
    private readonly HttpClient _http = httpClientFactory.CreateClient("GoogleOAuth");
    private readonly IJwtProvider _jwtProvider = jwtProvider;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly string _clientId =
            config["Google:ClientId"]
            ?? throw new InvalidOperationException("Google:ClientId not configured");

    private readonly string _clientSecret =
        config["Google:ClientSecret"]
        ?? throw new InvalidOperationException("Google:ClientSecret not configured");

    public async Task<string> AuthenticateAsync(string authorizationCode)
    {
        var tokenResponse = await ExchangeCodeForTokensAsync(authorizationCode);

        var payload = await GoogleJsonWebSignature.ValidateAsync(
            tokenResponse.IdToken,
            new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = [_clientId]
            }
        );

        var subject = payload.Subject;
        var email = payload.Email;
        var name = payload.Name;
        var refreshToken = tokenResponse.RefreshToken;

        var user = await _userRepository.GetUserByExternalSubjectAsync(subject);

        if (user is null)
        {
            user = new User(
                email: email,
                name: name,
                externalSubject: subject,
                externalRefreshToken: refreshToken
            );

            await _userRepository.CreateUserAsync(user);
        }
        else
        {
            user.ExternalRefreshToken = refreshToken;
            await _userRepository.UpdateUserAsync(user);
        }

        return _jwtProvider.GenerateToken(user.Id.ToString(), user.Email);
    }

    public async Task<GoogleTokenResponse> ExchangeCodeForTokensAsync(string code)
    {
        var content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["client_id"] = _clientId,
            ["client_secret"] = _clientSecret,
            ["code"] = code,
            ["grant_type"] = "authorization_code",
            ["redirect_uri"] = "postmessage",
        });

        var response = await _http.PostAsync("", content);


        var body = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            throw new Exception($"Google token error: {body}");

        return JsonSerializer.Deserialize<GoogleTokenResponse>(body)
               ?? throw new Exception("Invalid token response");
    }

    public async Task<string> GetAccessTokenAsync(string refreshToken)
    {
        var data = new Dictionary<string, string>
        {
            ["client_id"] = _clientId,
            ["client_secret"] = _clientSecret,
            ["refresh_token"] = refreshToken,
            ["grant_type"] = "refresh_token"
        };

        var response = await _http.PostAsync("", new FormUrlEncodedContent(data));
        var body = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            if (body.Contains("invalid_grant", StringComparison.OrdinalIgnoreCase))
                throw new GoogleRefreshTokenExpiredException("Refresh token expired or revoked by Google.");

            throw new Exception($"Google refresh error: {body}");
        }

        if (!response.IsSuccessStatusCode)
            throw new Exception($"Google refresh error: {body}");

        var json = JsonSerializer.Deserialize<JsonElement>(body);

        return json.GetProperty("access_token").GetString()
               ?? throw new Exception("Access token not returned");
    }

}


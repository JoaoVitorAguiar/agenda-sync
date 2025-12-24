using System.Text.Json;
using System.Text.Json.Serialization;
using AgendaSync.Entities;
using Google.Apis.Auth;

namespace AgendaSync.Services;

public class GoogleAuthService(IConfiguration config, HttpClient http, IJwtProvider jwtProvider, IUserRepository userRepository) : IAuthService
{
    private readonly IConfiguration _config = config;
    private readonly HttpClient _http = http;
    private readonly IJwtProvider _jwtProvider = jwtProvider;
    private readonly IUserRepository _userRepository = userRepository;
    public async Task<string> AuthenticateAsync(string authorizationCode)
    {
        var tokenResponse = await ExchangeCodeForTokensAsync(authorizationCode);

        var payload = await GoogleJsonWebSignature.ValidateAsync(
            tokenResponse.IdToken,
            new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = new[] { _config["Google:ClientId"]! }
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

    private async Task<GoogleTokenResponse> ExchangeCodeForTokensAsync(string code)
    {
        var content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["client_id"] = _config["Google:ClientId"]!,
            ["client_secret"] = _config["Google:ClientSecret"]!,
            ["code"] = code,
            ["grant_type"] = "authorization_code",
            ["redirect_uri"] = "postmessage",
        });

        var response = await _http.PostAsync(
            "https://oauth2.googleapis.com/token",
            content
        );

        var body = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            throw new Exception($"Google token error: {body}");

        return JsonSerializer.Deserialize<GoogleTokenResponse>(body)
               ?? throw new Exception("Invalid token response");
    }
}


public record GoogleTokenResponse(
    [property: JsonPropertyName("access_token")] string AccessToken,
    [property: JsonPropertyName("expires_in")] int ExpiresIn,
    [property: JsonPropertyName("refresh_token")] string? RefreshToken,
    [property: JsonPropertyName("scope")] string Scope,
    [property: JsonPropertyName("token_type")] string TokenType,
    [property: JsonPropertyName("id_token")] string IdToken
);



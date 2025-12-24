using System.Text.Json;
using System.Text.Json.Serialization;
using Google.Apis.Auth;

namespace AgendaSync.Services;

public class GoogleAuthService(IConfiguration config, HttpClient http, IJwtProvider jwtProvider) : IAuthService
{
    private readonly IConfiguration _config = config;
    private readonly HttpClient _http = http;
    private readonly IJwtProvider _jwtProvider = jwtProvider;

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

        var userId = payload.Subject;
        var email = payload.Email;

        // TODO: save refresh token in the database 

        return _jwtProvider.GenerateToken(userId, email);
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



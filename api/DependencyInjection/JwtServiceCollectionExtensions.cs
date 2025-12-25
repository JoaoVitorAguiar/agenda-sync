using System.Text;
using AgendaSync.Services;
using AgendaSync.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace AgendaSync.DependencyInjection;

public static class JwtServiceCollectionExtensions
{
    public static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var jwt = configuration.GetSection("Jwt");
        var secretKey = jwt["SecretKey"]!;
        var issuer = jwt["Issuer"]!;
        var audience = jwt["Audience"]!;
        var expiry = int.Parse(jwt["ExpiryInMinutes"]!);

        services.AddSingleton<IJwtProvider>(
            _ => new JwtProvider(secretKey, issuer, audience, expiry)
        );

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(o =>
            {
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = issuer,
                    ValidateAudience = true,
                    ValidAudience = audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

        services.AddAuthorization();
        return services;
    }
}

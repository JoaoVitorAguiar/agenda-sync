using Scalar.AspNetCore;

namespace AgendaSync.DependencyInjection;

public static class ScalarServiceCollectionExtensions
{
    public static void UseScalarDocs(this WebApplication app)
    {
        if (!app.Environment.IsDevelopment()) return;

        app.MapOpenApi();
        app.MapScalarApiReference("/api-docs", options =>
        {
            options.Authentication = new()
            {
                PreferredSecuritySchemes = new[] { "Bearer" },
                SecuritySchemes = new Dictionary<string, ScalarSecurityScheme>
                {
                    ["Bearer"] = new ScalarHttpSecurityScheme { Token = "" }
                }
            };
        });
    }
}

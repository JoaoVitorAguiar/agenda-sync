using AgendaSync.OpenApi;

namespace AgendaSync.DependencyInjection;

public static class OpenApiServiceCollectionExtensions
{
    public static IServiceCollection AddOpenApiWithBearer(this IServiceCollection services)
    {
        services.AddOpenApi(options =>
        {
            options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
        });
        return services;
    }
}

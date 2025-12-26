using AgendaSync.Repositories;
using AgendaSync.Repositories.Interfaces;
using AgendaSync.Security;
using AgendaSync.Services;
using AgendaSync.Services.Interfaces;

namespace AgendaSync.DependencyInjection;

public static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, GoogleAuthService>();
        services.AddScoped<IEventService, EventService>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<AuthUser>(sp =>
        {
            var http = sp.GetRequiredService<IHttpContextAccessor>().HttpContext;

            if (http?.User?.Identity is null || http.User.Identity.IsAuthenticated == false)
                throw new UnauthorizedAccessException("No authenticated user in context.");

            return new AuthUser(http.User);
        });


        return services;
    }
}

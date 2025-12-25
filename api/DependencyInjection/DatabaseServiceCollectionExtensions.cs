using Microsoft.EntityFrameworkCore;
using AgendaSync.Data;

namespace AgendaSync.DependencyInjection;

public static class DatabaseServiceCollectionExtensions
{
    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AgendaSyncDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"))
        );

        return services;
    }
}

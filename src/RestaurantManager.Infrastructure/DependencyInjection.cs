using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RestaurantManager.Infrastructure.Persistence;

namespace RestaurantManager.Infrastructure;

/// <summary>
/// Extension methods for setting up Infrastructure services in the DI container.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Register DbContext with SQL Server
        services.AddDbContext<RestaurantDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                builder => builder.MigrationsAssembly(typeof(RestaurantDbContext).Assembly.FullName)));

        // Register additional infrastructure services here
        // Example: services.AddScoped<IApplicationDbContext, RestaurantDbContext>();

        return services;
    }
}

using Microsoft.Extensions.DependencyInjection;

namespace RestaurantManager.Application;

/// <summary>
/// Extension methods for setting up Application services in the DI container.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Register AutoMapper
        services.AddAutoMapper(cfg => cfg.AddMaps(typeof(DependencyInjection).Assembly));

        // Register FluentValidation validators
        // services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        // Register MediatR if using CQRS pattern
        // services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));

        return services;
    }
}

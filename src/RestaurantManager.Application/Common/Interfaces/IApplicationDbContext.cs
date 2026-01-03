namespace RestaurantManager.Application.Common.Interfaces;

/// <summary>
/// Abstraction for the database context.
/// Allows the Application layer to remain independent of Infrastructure concerns.
/// </summary>
public interface IApplicationDbContext
{
    // Add DbSet properties here that match your DbContext
    // Example: DbSet<Restaurant> Restaurants { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

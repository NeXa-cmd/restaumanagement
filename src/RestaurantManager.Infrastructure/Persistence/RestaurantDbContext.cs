using Microsoft.EntityFrameworkCore;
using RestaurantManager.Domain.Entities;

namespace RestaurantManager.Infrastructure.Persistence;

/// <summary>
/// The main database context for the RestaurantManager application.
/// Configured to work with Azure SQL Server.
/// </summary>
public class RestaurantDbContext : DbContext
{
    public RestaurantDbContext(DbContextOptions<RestaurantDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<MenuItem> MenuItems => Set<MenuItem>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<Cart> Carts => Set<Cart>();
    public DbSet<CartItem> CartItems => Set<CartItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.Email).HasMaxLength(255).IsRequired();
            entity.Property(e => e.PasswordHash).HasMaxLength(255).IsRequired();
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.Role).HasConversion<int>();
        });

        // MenuItem configuration
        modelBuilder.Entity<MenuItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Price).HasPrecision(10, 2);
            entity.Property(e => e.Category).HasMaxLength(50);
            entity.Property(e => e.ImageUrl).HasMaxLength(500);
        });

        // Category configuration
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(50).IsRequired();
        });

        // Order configuration
        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.OrderNumber).IsUnique();
            entity.Property(e => e.OrderNumber).HasMaxLength(20).IsRequired();
            entity.Property(e => e.TotalAmount).HasPrecision(10, 2);
            entity.HasOne(e => e.User)
                  .WithMany(u => u.Orders)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // OrderItem configuration
        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.UnitPrice).HasPrecision(10, 2);
            entity.Property(e => e.TotalPrice).HasPrecision(10, 2);
            entity.HasOne(e => e.Order)
                  .WithMany(o => o.OrderItems)
                  .HasForeignKey(e => e.OrderId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.MenuItem)
                  .WithMany(m => m.OrderItems)
                  .HasForeignKey(e => e.MenuItemId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Cart configuration
        modelBuilder.Entity<Cart>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.User)
                  .WithOne(u => u.Cart)
                  .HasForeignKey<Cart>(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // CartItem configuration
        modelBuilder.Entity<CartItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Cart)
                  .WithMany(c => c.CartItems)
                  .HasForeignKey(e => e.CartId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.MenuItem)
                  .WithMany(m => m.CartItems)
                  .HasForeignKey(e => e.MenuItemId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Seed Admin user
        modelBuilder.Entity<User>().HasData(new User
        {
            Id = 1,
            Email = "admin@restaurant.com",
            PasswordHash = "admin123", // In production, use proper hashing
            FullName = "Administrator",
            Role = UserRole.Admin,
            IsActive = true,
            CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        });

        // Seed Cashier user
        modelBuilder.Entity<User>().HasData(new User
        {
            Id = 2,
            Email = "cashier@restaurant.com",
            PasswordHash = "cashier123", // In production, use proper hashing
            FullName = "Cashier User",
            Role = UserRole.Cashier,
            IsActive = true,
            CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        });

        // Seed categories
        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Appetizers", DisplayOrder = 1, IsActive = true },
            new Category { Id = 2, Name = "Main Course", DisplayOrder = 2, IsActive = true },
            new Category { Id = 3, Name = "Desserts", DisplayOrder = 3, IsActive = true },
            new Category { Id = 4, Name = "Drinks", DisplayOrder = 4, IsActive = true }
        );

        // Seed menu items with image URLs from Unsplash
        var seedDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        modelBuilder.Entity<MenuItem>().HasData(
            // Appetizers
            new MenuItem { Id = 1, Name = "Spring Rolls", Description = "Crispy vegetable spring rolls served with sweet chili sauce", Price = 8.99m, Category = "Appetizers", ImageUrl = "https://images.unsplash.com/photo-1544025162-d76694265947?w=400", IsAvailable = true, CreatedAt = seedDate },
            new MenuItem { Id = 2, Name = "Garlic Bread", Description = "Toasted artisan bread with garlic butter and herbs", Price = 5.99m, Category = "Appetizers", ImageUrl = "https://images.unsplash.com/photo-1619535860434-ba1d8fa12536?w=400", IsAvailable = true, CreatedAt = seedDate },
            new MenuItem { Id = 3, Name = "Caesar Salad", Description = "Fresh romaine lettuce with parmesan and croutons", Price = 9.99m, Category = "Appetizers", ImageUrl = "https://images.unsplash.com/photo-1550304943-4f24f54ddde9?w=400", IsAvailable = true, CreatedAt = seedDate },
            new MenuItem { Id = 4, Name = "Soup of the Day", Description = "Chef's daily special soup selection", Price = 6.99m, Category = "Appetizers", ImageUrl = "https://images.unsplash.com/photo-1547592166-23ac45744acd?w=400", IsAvailable = false, CreatedAt = seedDate },
            
            // Main Course
            new MenuItem { Id = 5, Name = "Grilled Salmon", Description = "Atlantic salmon with lemon herb butter", Price = 24.99m, Category = "Main Course", ImageUrl = "https://images.unsplash.com/photo-1467003909585-2f8a72700288?w=400", IsAvailable = true, CreatedAt = seedDate },
            new MenuItem { Id = 6, Name = "Ribeye Steak", Description = "12oz prime ribeye cooked to perfection", Price = 32.99m, Category = "Main Course", ImageUrl = "https://images.unsplash.com/photo-1600891964092-4316c288032e?w=400", IsAvailable = true, CreatedAt = seedDate },
            new MenuItem { Id = 7, Name = "Chicken Parmesan", Description = "Breaded chicken breast with marinara and mozzarella", Price = 18.99m, Category = "Main Course", ImageUrl = "https://images.unsplash.com/photo-1632778149955-e80f8ceca2e8?w=400", IsAvailable = true, CreatedAt = seedDate },
            new MenuItem { Id = 8, Name = "Vegetable Pasta", Description = "Penne with seasonal vegetables in garlic sauce", Price = 15.99m, Category = "Main Course", ImageUrl = "https://images.unsplash.com/photo-1621996346565-e3dbc646d9a9?w=400", IsAvailable = true, CreatedAt = seedDate },
            new MenuItem { Id = 9, Name = "Lamb Chops", Description = "Herb-crusted New Zealand lamb chops", Price = 29.99m, Category = "Main Course", ImageUrl = "https://images.unsplash.com/photo-1614398751058-eb2e0bf63e53?w=400", IsAvailable = true, CreatedAt = seedDate },
            
            // Desserts
            new MenuItem { Id = 10, Name = "Chocolate Lava Cake", Description = "Warm chocolate cake with molten center", Price = 8.99m, Category = "Desserts", ImageUrl = "https://images.unsplash.com/photo-1624353365286-3f8d62daad51?w=400", IsAvailable = true, CreatedAt = seedDate },
            new MenuItem { Id = 11, Name = "New York Cheesecake", Description = "Classic creamy cheesecake with berry compote", Price = 9.99m, Category = "Desserts", ImageUrl = "https://images.unsplash.com/photo-1567171466295-4afa63d45416?w=400", IsAvailable = true, CreatedAt = seedDate },
            new MenuItem { Id = 12, Name = "Tiramisu", Description = "Traditional Italian coffee-flavored dessert", Price = 10.99m, Category = "Desserts", ImageUrl = "https://images.unsplash.com/photo-1571877227200-a0d98ea607e9?w=400", IsAvailable = true, CreatedAt = seedDate },
            
            // Drinks
            new MenuItem { Id = 13, Name = "Fresh Lemonade", Description = "House-made lemonade with fresh mint", Price = 4.99m, Category = "Drinks", ImageUrl = "https://images.unsplash.com/photo-1621263764928-df1444c5e859?w=400", IsAvailable = true, CreatedAt = seedDate },
            new MenuItem { Id = 14, Name = "Iced Tea", Description = "Refreshing brewed iced tea", Price = 3.99m, Category = "Drinks", ImageUrl = "https://images.unsplash.com/photo-1556679343-c7306c1976bc?w=400", IsAvailable = true, CreatedAt = seedDate },
            new MenuItem { Id = 15, Name = "Espresso", Description = "Double shot Italian espresso", Price = 3.49m, Category = "Drinks", ImageUrl = "https://images.unsplash.com/photo-1510707577719-ae7c14805e3a?w=400", IsAvailable = true, CreatedAt = seedDate },
            new MenuItem { Id = 16, Name = "House Wine", Description = "Glass of house red or white wine", Price = 8.99m, Category = "Drinks", ImageUrl = "https://images.unsplash.com/photo-1510812431401-41d2bd2722f3?w=400", IsAvailable = true, CreatedAt = seedDate }
        );
    }
}

#!/bin/bash

# =============================================================================
# RestaurantManager - Clean Architecture Solution Scaffold Script
# =============================================================================
# This script creates a .NET solution following Clean Architecture principles
# with Razor Pages for the Web layer.
# =============================================================================

set -e  # Exit on any error

echo "🚀 Starting RestaurantManager Solution Scaffold..."
echo "=================================================="

# -----------------------------------------------------------------------------
# 1. Create Solution
# -----------------------------------------------------------------------------
echo ""
echo "📁 Creating Solution..."
dotnet new sln -n RestaurantManager

# -----------------------------------------------------------------------------
# 2. Create Projects
# -----------------------------------------------------------------------------
echo ""
echo "📦 Creating Projects..."

# Domain Layer - Core business entities and logic (no dependencies)
echo "  → Creating RestaurantManager.Domain (Class Library)..."
dotnet new classlib -n RestaurantManager.Domain -o src/RestaurantManager.Domain
rm -f src/RestaurantManager.Domain/Class1.cs

# Application Layer - Use cases, interfaces, DTOs
echo "  → Creating RestaurantManager.Application (Class Library)..."
dotnet new classlib -n RestaurantManager.Application -o src/RestaurantManager.Application
rm -f src/RestaurantManager.Application/Class1.cs

# Infrastructure Layer - External concerns (DB, APIs, etc.)
echo "  → Creating RestaurantManager.Infrastructure (Class Library)..."
dotnet new classlib -n RestaurantManager.Infrastructure -o src/RestaurantManager.Infrastructure
rm -f src/RestaurantManager.Infrastructure/Class1.cs

# Web Layer - Razor Pages UI
echo "  → Creating RestaurantManager.Web (Razor Pages)..."
dotnet new webapp -n RestaurantManager.Web -o src/RestaurantManager.Web

# -----------------------------------------------------------------------------
# 3. Add Projects to Solution
# -----------------------------------------------------------------------------
echo ""
echo "🔗 Adding Projects to Solution..."

# Detect solution file (supports both .sln and .slnx formats)
SLN_FILE=$(ls RestaurantManager.sln 2>/dev/null || ls RestaurantManager.slnx 2>/dev/null)
echo "  → Using solution file: $SLN_FILE"

dotnet sln "$SLN_FILE" add src/RestaurantManager.Domain/RestaurantManager.Domain.csproj
dotnet sln "$SLN_FILE" add src/RestaurantManager.Application/RestaurantManager.Application.csproj
dotnet sln "$SLN_FILE" add src/RestaurantManager.Infrastructure/RestaurantManager.Infrastructure.csproj
dotnet sln "$SLN_FILE" add src/RestaurantManager.Web/RestaurantManager.Web.csproj

# -----------------------------------------------------------------------------
# 4. Establish Project References (Dependency Graph)
# -----------------------------------------------------------------------------
echo ""
echo "🔀 Setting Up Project References..."

# Application depends on Domain
echo "  → Application → Domain"
dotnet add src/RestaurantManager.Application/RestaurantManager.Application.csproj reference src/RestaurantManager.Domain/RestaurantManager.Domain.csproj

# Infrastructure depends on Application and Domain
echo "  → Infrastructure → Application"
dotnet add src/RestaurantManager.Infrastructure/RestaurantManager.Infrastructure.csproj reference src/RestaurantManager.Application/RestaurantManager.Application.csproj
echo "  → Infrastructure → Domain"
dotnet add src/RestaurantManager.Infrastructure/RestaurantManager.Infrastructure.csproj reference src/RestaurantManager.Domain/RestaurantManager.Domain.csproj

# Web depends on Application and Infrastructure
echo "  → Web → Application"
dotnet add src/RestaurantManager.Web/RestaurantManager.Web.csproj reference src/RestaurantManager.Application/RestaurantManager.Application.csproj
echo "  → Web → Infrastructure"
dotnet add src/RestaurantManager.Web/RestaurantManager.Web.csproj reference src/RestaurantManager.Infrastructure/RestaurantManager.Infrastructure.csproj

# -----------------------------------------------------------------------------
# 5. Install NuGet Packages
# -----------------------------------------------------------------------------
echo ""
echo "📥 Installing NuGet Packages..."

# Infrastructure packages (EF Core for Azure SQL)
echo "  → Installing EF Core packages to Infrastructure..."
dotnet add src/RestaurantManager.Infrastructure/RestaurantManager.Infrastructure.csproj package Microsoft.EntityFrameworkCore.SqlServer
dotnet add src/RestaurantManager.Infrastructure/RestaurantManager.Infrastructure.csproj package Microsoft.EntityFrameworkCore.Tools

# Application packages (FluentValidation & AutoMapper)
echo "  → Installing FluentValidation and AutoMapper to Application..."
dotnet add src/RestaurantManager.Application/RestaurantManager.Application.csproj package FluentValidation
dotnet add src/RestaurantManager.Application/RestaurantManager.Application.csproj package AutoMapper

# -----------------------------------------------------------------------------
# 6. Create Basic Folder Structure
# -----------------------------------------------------------------------------
echo ""
echo "📂 Creating Folder Structure..."

# Domain folders
mkdir -p src/RestaurantManager.Domain/Entities
mkdir -p src/RestaurantManager.Domain/ValueObjects
mkdir -p src/RestaurantManager.Domain/Enums
mkdir -p src/RestaurantManager.Domain/Exceptions

# Application folders
mkdir -p src/RestaurantManager.Application/Common/Interfaces
mkdir -p src/RestaurantManager.Application/Common/Mappings
mkdir -p src/RestaurantManager.Application/Common/Behaviours
mkdir -p src/RestaurantManager.Application/Features

# Infrastructure folders
mkdir -p src/RestaurantManager.Infrastructure/Persistence
mkdir -p src/RestaurantManager.Infrastructure/Persistence/Configurations
mkdir -p src/RestaurantManager.Infrastructure/Services

# -----------------------------------------------------------------------------
# 7. Scaffold Basic Files
# -----------------------------------------------------------------------------
echo ""
echo "📝 Scaffolding Basic Files..."

# Create RestaurantDbContext in Infrastructure
cat > src/RestaurantManager.Infrastructure/Persistence/RestaurantDbContext.cs << 'EOF'
using Microsoft.EntityFrameworkCore;

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

    // Add DbSet properties for your entities here
    // Example: public DbSet<Restaurant> Restaurants => Set<Restaurant>();
    // Example: public DbSet<MenuItem> MenuItems => Set<MenuItem>();
    // Example: public DbSet<Reservation> Reservations => Set<Reservation>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all entity configurations from the current assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(RestaurantDbContext).Assembly);
    }
}
EOF

# Create IApplicationDbContext interface in Application
cat > src/RestaurantManager.Application/Common/Interfaces/IApplicationDbContext.cs << 'EOF'
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
EOF

# Create a sample Entity in Domain
cat > src/RestaurantManager.Domain/Entities/Restaurant.cs << 'EOF'
namespace RestaurantManager.Domain.Entities;

/// <summary>
/// Represents a restaurant in the system.
/// </summary>
public class Restaurant
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
EOF

# Create DependencyInjection for Infrastructure
cat > src/RestaurantManager.Infrastructure/DependencyInjection.cs << 'EOF'
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
EOF

# Create DependencyInjection for Application
cat > src/RestaurantManager.Application/DependencyInjection.cs << 'EOF'
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
        services.AddAutoMapper(typeof(DependencyInjection).Assembly);

        // Register FluentValidation validators
        // services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        // Register MediatR if using CQRS pattern
        // services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));

        return services;
    }
}
EOF

# -----------------------------------------------------------------------------
# 8. Build Solution to Verify
# -----------------------------------------------------------------------------
echo ""
echo "🔨 Building Solution..."
dotnet build "$SLN_FILE"

# -----------------------------------------------------------------------------
# Done!
# -----------------------------------------------------------------------------
echo ""
echo "=================================================="
echo "✅ RestaurantManager Solution Created Successfully!"
echo "=================================================="
echo ""
echo "📊 Project Structure:"
echo ""
echo "  RestaurantManager.sln"
echo "  └── src/"
echo "      ├── RestaurantManager.Domain/        (Core entities, no dependencies)"
echo "      ├── RestaurantManager.Application/   (Use cases, depends on Domain)"
echo "      ├── RestaurantManager.Infrastructure/(Data access, depends on Application & Domain)"
echo "      └── RestaurantManager.Web/           (Razor Pages UI, depends on Application & Infrastructure)"
echo ""
echo "📦 Installed Packages:"
echo "  • Infrastructure: Microsoft.EntityFrameworkCore.SqlServer, Microsoft.EntityFrameworkCore.Tools"
echo "  • Application: FluentValidation, AutoMapper"
echo ""
echo "🚀 Next Steps:"
echo "  1. Update connection string in appsettings.json"
echo "  2. Add entity configurations in Infrastructure/Persistence/Configurations"
echo "  3. Create your domain entities in Domain/Entities"
echo "  4. Add features in Application/Features"
echo "  5. Run: dotnet ef migrations add InitialCreate -p src/RestaurantManager.Infrastructure -s src/RestaurantManager.Web"
echo ""

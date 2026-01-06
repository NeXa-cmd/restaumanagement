# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy solution and project files
COPY ["src/RestaurantManager.Web/RestaurantManager.Web.csproj", "src/RestaurantManager.Web/"]
COPY ["src/RestaurantManager.Infrastructure/RestaurantManager.Infrastructure.csproj", "src/RestaurantManager.Infrastructure/"]
COPY ["src/RestaurantManager.Domain/RestaurantManager.Domain.csproj", "src/RestaurantManager.Domain/"]
COPY ["src/RestaurantManager.Application/RestaurantManager.Application.csproj", "src/RestaurantManager.Application/"]

# Restore dependencies
RUN dotnet restore "src/RestaurantManager.Web/RestaurantManager.Web.csproj"

# Copy all source code
COPY . .

# Build the application
WORKDIR "/src/src/RestaurantManager.Web"
RUN dotnet build "RestaurantManager.Web.csproj" -c Release -o /app/build

# Stage 2: Publish
FROM build AS publish
RUN dotnet publish "RestaurantManager.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 3: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

# Install curl for healthcheck
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# Copy published files
COPY --from=publish /app/publish .

# Expose ports
EXPOSE 8080
EXPOSE 8081

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
  CMD curl -f http://localhost:8080/health || exit 1

# Entry point
ENTRYPOINT ["dotnet", "RestaurantManager.Web.dll"]

# Build stage
ENTRYPOINT ["dotnet", "RestaurantManager.Web.dll"]
# Run the application

ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:8080
# Set environment variables

COPY --from=publish /app/publish .
# Copy published files from publish stage

EXPOSE 8081
EXPOSE 8080
WORKDIR /app
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
# Runtime stage

RUN dotnet publish "RestaurantManager.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false
FROM build AS publish
# Publish stage

RUN dotnet build "RestaurantManager.Web.csproj" -c Release -o /app/build
WORKDIR "/src/RestaurantManager.Web"
# Build the application

COPY src/ .
# Copy all source files

RUN dotnet restore "RestaurantManager.Web/RestaurantManager.Web.csproj"
# Restore dependencies

COPY ["src/RestaurantManager.Infrastructure/RestaurantManager.Infrastructure.csproj", "RestaurantManager.Infrastructure/"]
COPY ["src/RestaurantManager.Domain/RestaurantManager.Domain.csproj", "RestaurantManager.Domain/"]
COPY ["src/RestaurantManager.Application/RestaurantManager.Application.csproj", "RestaurantManager.Application/"]
COPY ["src/RestaurantManager.Web/RestaurantManager.Web.csproj", "RestaurantManager.Web/"]
# Copy solution and project files

WORKDIR /src
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build


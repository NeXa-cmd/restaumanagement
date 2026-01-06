# Restaurant Manager - Docker Setup

## Prerequisites
- Docker Desktop installed
- Docker Compose installed

## Quick Start

### 1. Production Mode (Recommended)

Build and run the entire application with SQL Server:

```bash
docker-compose up -d
```

The application will be available at: **http://localhost:5000**

### 2. Development Mode

Run with hot reload enabled:

```bash
docker-compose -f docker-compose.dev.yml up -d
```

The application will be available at: **http://localhost:5001**

## Available Commands

### Build the Docker image
```bash
docker build -t restaurant-manager .
```

### Run with Docker Compose
```bash
# Start all services
docker-compose up -d

# View logs
docker-compose logs -f

# Stop all services
docker-compose down

# Stop and remove volumes (deletes database)
docker-compose down -v
```

### Run single container (without SQL Server)
```bash
docker run -d -p 5000:8080 \
  -e ConnectionStrings__DefaultConnection="YourConnectionString" \
  restaurant-manager
```

## Services

### Web Application
- **Port**: 5000 (maps to container port 8080)
- **Image**: Built from Dockerfile
- **Health Check**: http://localhost:5000/health

### SQL Server
- **Port**: 1433
- **SA Password**: `YourStrong@Password123` (change in production!)
- **Database**: RestaurantManager

## Login Credentials

### Admin Account
- **Email**: `admin@restaurant.com`
- **Password**: `admin123`

### Cashier Account
- **Email**: `cashier@restaurant.com`
- **Password**: `cashier123`

## Environment Variables

You can customize the application using environment variables:

```bash
# In docker-compose.yml, modify:
environment:
  - ASPNETCORE_ENVIRONMENT=Production
  - ConnectionStrings__DefaultConnection=Server=sqlserver;Database=RestaurantManager;...
```

## Volumes

### Database Volume
The SQL Server data is persisted in a Docker volume named `sqlserver-data`.

To backup:
```bash
docker run --rm -v sqlserver-data:/data -v $(pwd):/backup ubuntu tar czf /backup/backup.tar.gz /data
```

To restore:
```bash
docker run --rm -v sqlserver-data:/data -v $(pwd):/backup ubuntu tar xzf /backup/backup.tar.gz -C /
```

## Troubleshooting

### Check container status
```bash
docker-compose ps
```

### View logs
```bash
# All services
docker-compose logs

# Specific service
docker-compose logs webapp
docker-compose logs sqlserver
```

### Restart services
```bash
docker-compose restart
```

### Reset everything
```bash
docker-compose down -v
docker-compose up -d
```

### Connect to SQL Server container
```bash
docker exec -it restaurant-db /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "YourStrong@Password123" -C
```

## Production Deployment

### 1. Update passwords in docker-compose.yml
Change the default SA password to a strong password.

### 2. Use environment files
Create a `.env` file:
```env
SA_PASSWORD=YourProductionPassword
ASPNETCORE_ENVIRONMENT=Production
```

### 3. Enable HTTPS (optional)
Update the Dockerfile to expose port 8081 and configure certificates.

### 4. Deploy to cloud
- **Azure**: Use Azure Container Apps or AKS
- **AWS**: Use ECS or EKS
- **Google Cloud**: Use Cloud Run or GKE

## Network

All services run on the `restaurant-network` bridge network, allowing them to communicate using service names.

## Health Checks

The web application includes a health check endpoint that Docker monitors:
- **Interval**: 30 seconds
- **Timeout**: 3 seconds
- **Retries**: 3

## Stopping the Application

```bash
# Stop but keep data
docker-compose stop

# Stop and remove containers (keeps volumes)
docker-compose down

# Stop and remove everything including data
docker-compose down -v
```

## Updating the Application

```bash
# Rebuild and restart
docker-compose up -d --build
```

## Performance Tips

1. **Memory**: Allocate at least 2GB RAM to Docker
2. **CPU**: Allocate at least 2 CPUs
3. **Storage**: Ensure sufficient disk space for SQL Server data

## Security Notes

?? **Important for Production:**
- Change default passwords
- Use secrets management (Docker secrets, Azure Key Vault, etc.)
- Enable HTTPS
- Configure firewall rules
- Use non-root user in container
- Scan images for vulnerabilities

## Support

For issues or questions:
1. Check container logs: `docker-compose logs`
2. Verify network connectivity: `docker network inspect restaurant-network`
3. Check database connection: Connect to SQL Server container

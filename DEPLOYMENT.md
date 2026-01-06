# Restaurant Manager - Docker Deployment Guide

This guide will help you deploy the Restaurant Manager application using Docker on your VPS.

## Prerequisites

- Docker installed on your VPS
- Docker Compose installed (optional, but recommended)
- Access to Azure SQL Database (olkad.database.windows.net)

## Quick Start

### Option 1: Using Docker Compose (Recommended)

1. Clone/upload the project to your VPS
2. Navigate to the project root directory
3. Build and run the container:

```bash
docker-compose up -d --build
```

The application will be available at:
- HTTP: `http://your-vps-ip:8080`

### Option 2: Using Docker CLI

1. Build the Docker image:

```bash
docker build -t restaurant-manager:latest .
```

2. Run the container:

```bash
docker run -d \
  --name restaurant-manager \
  -p 8080:8080 \
  -e ASPNETCORE_ENVIRONMENT=Production \
  -e "ConnectionStrings__DefaultConnection=Server=tcp:olkad.database.windows.net,1433;Initial Catalog=RestaurantManager;Persist Security Info=False;User ID=admi;Password=Olkilolkil12@;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;" \
  --restart unless-stopped \
  restaurant-manager:latest
```

## Managing the Application

### View logs
```bash
docker-compose logs -f
# or
docker logs -f restaurant-manager
```

### Stop the application
```bash
docker-compose down
# or
docker stop restaurant-manager
```

### Restart the application
```bash
docker-compose restart
# or
docker restart restaurant-manager
```

### Update the application
```bash
docker-compose down
docker-compose up -d --build
# or
docker stop restaurant-manager
docker rm restaurant-manager
docker build -t restaurant-manager:latest .
# then run the docker run command again
```

## Security Considerations

### For Production Deployment:

1. **Use Environment Variables**: Never hardcode sensitive credentials in files. Use environment variables or secrets management.

2. **Create a .env file** (don't commit this to Git):
```bash
cp .env.example .env
# Edit .env with your actual credentials
```

3. **Update docker-compose.yml** to use the .env file:
```yaml
services:
  restaurantmanager:
    env_file:
      - .env
```

4. **Set up HTTPS**: For production, you should set up a reverse proxy (like Nginx or Caddy) with SSL certificates.

Example Nginx configuration:
```nginx
server {
    listen 80;
    server_name your-domain.com;
    
    location / {
        proxy_pass http://localhost:8080;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection keep-alive;
        proxy_set_header Host $host;
        proxy_cache_bypass $http_upgrade;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }
}
```

5. **Firewall Configuration**: Only expose necessary ports
```bash
# Allow SSH
sudo ufw allow 22/tcp

# Allow HTTP and HTTPS
sudo ufw allow 80/tcp
sudo ufw allow 443/tcp

# Enable firewall
sudo ufw enable
```

## Database Migrations

The application automatically applies database migrations on startup. The first time you run the container, it will:
1. Connect to your Azure SQL Database
2. Apply all pending migrations
3. Create the necessary tables

If you need to run migrations manually:
```bash
docker exec -it restaurant-manager dotnet ef database update
```

## Troubleshooting

### Container won't start
Check logs:
```bash
docker logs restaurant-manager
```

### Database connection issues
1. Verify Azure SQL firewall rules allow connections from your VPS IP
2. Test the connection string
3. Check if the database exists

### Port already in use
Change the port mapping in docker-compose.yml:
```yaml
ports:
  - "8080:8080"  # Change first number to available port
```

## Health Checks

To verify the application is running:
```bash
curl http://localhost:8080
```

## Monitoring

View resource usage:
```bash
docker stats restaurant-manager
```

## Backup

To backup the Docker image:
```bash
docker save restaurant-manager:latest | gzip > restaurant-manager-backup.tar.gz
```

To restore:
```bash
docker load < restaurant-manager-backup.tar.gz
```

## CI/CD Integration

You can automate deployments using GitHub Actions, GitLab CI, or other CI/CD tools. Example GitHub Actions workflow is included in `.github/workflows/deploy.yml`.

## Support

For issues or questions, check the application logs first:
```bash
docker-compose logs -f restaurant-manager
```


# ?? Restaurant Manager - Docker Deployment Guide

## ? What Was Created

Your project is now fully containerized with the following files:

1. **Dockerfile** - Multi-stage build for the application
2. **docker-compose.yml** - Production configuration
3. **docker-compose.dev.yml** - Development configuration
4. **.dockerignore** - Excludes unnecessary files from the image
5. **start-docker.bat** - Windows startup script
6. **start-docker.sh** - Linux/Mac startup script
7. **Health endpoint** - `/health` for monitoring

---

## ?? Quick Start

### Option 1: Using the Startup Script (Easiest)

**Windows:**
```bash
start-docker.bat
```

**Linux/Mac:**
```bash
chmod +x start-docker.sh
./start-docker.sh
```

### Option 2: Manual Docker Compose

```bash
# Build and start
docker-compose up -d

# View logs
docker-compose logs -f

# Stop
docker-compose down
```

---

## ?? What's Included

### Services

1. **Web Application** (restaurant-app)
   - Port: `5000` ? `http://localhost:5000`
   - Built from your source code
   - Includes health checks
   - Auto-restarts on failure

2. **SQL Server** (restaurant-db)
   - Port: `1433`
   - Database: `RestaurantManager`
   - User: `sa`
   - Password: `YourStrong@Password123` ?? (Change this!)
   - Persistent data storage

### Network
- Custom bridge network: `restaurant-network`
- Services can communicate using service names

### Volumes
- `sqlserver-data` - Persists database across restarts

---

## ?? Default Credentials

### SQL Server
- **User**: `sa`
- **Password**: `YourStrong@Password123`
- **Server**: `localhost,1433`
- **Database**: `RestaurantManager`

### Application
- **Admin**: `admin@restaurant.com` / `admin123`
- **Cashier**: `cashier@restaurant.com` / `cashier123`

---

## ?? Common Commands

### Start/Stop
```bash
# Start all services
docker-compose up -d

# Stop all services
docker-compose stop

# Stop and remove containers
docker-compose down

# Stop and remove volumes (deletes data!)
docker-compose down -v
```

### Logs
```bash
# All logs
docker-compose logs

# Follow logs
docker-compose logs -f

# Specific service
docker-compose logs webapp
docker-compose logs sqlserver
```

### Rebuild
```bash
# Rebuild after code changes
docker-compose up -d --build

# Force rebuild
docker-compose build --no-cache
docker-compose up -d
```

### Status
```bash
# Check running containers
docker-compose ps

# Check resource usage
docker stats
```

---

## ?? Configuration

### Change SQL Server Password

1. Edit `docker-compose.yml`:
```yaml
environment:
  - SA_PASSWORD=YourNewStrongPassword
```

2. Update connection string:
```yaml
- ConnectionStrings__DefaultConnection=Server=sqlserver;Database=RestaurantManager;User Id=sa;Password=YourNewStrongPassword;TrustServerCertificate=True;
```

3. Restart:
```bash
docker-compose down -v
docker-compose up -d
```

### Change Application Port

Edit `docker-compose.yml`:
```yaml
ports:
  - "8080:8080"  # Change first number to desired port
```

### Environment Variables

Create `.env` file in the root directory:
```env
SA_PASSWORD=YourPassword
ASPNETCORE_ENVIRONMENT=Production
```

Update `docker-compose.yml` to use:
```yaml
environment:
  - SA_PASSWORD=${SA_PASSWORD}
```

---

## ?? Troubleshooting

### Container won't start

```bash
# Check logs
docker-compose logs

# Check if ports are in use
netstat -ano | findstr :5000
netstat -ano | findstr :1433
```

### Database connection failed

```bash
# Check SQL Server health
docker exec -it restaurant-db /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "YourStrong@Password123" -C -Q "SELECT @@VERSION"

# Check network
docker network inspect restaurant-network
```

### Application errors

```bash
# View real-time logs
docker-compose logs -f webapp

# Restart the app
docker-compose restart webapp
```

### Reset everything

```bash
# Nuclear option - removes everything
docker-compose down -v
docker system prune -a
docker volume prune
docker-compose up -d
```

---

## ?? Health Checks

### Application Health
```bash
curl http://localhost:5000/health
```

Expected response:
```json
{
  "status": "healthy",
  "timestamp": "2025-01-01T12:00:00.000Z"
}
```

### Database Health
```bash
docker exec -it restaurant-db /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "YourStrong@Password123" -C -Q "SELECT 1"
```

---

## ?? Deployment Options

### 1. Local Development
Use `docker-compose.dev.yml` for hot reload:
```bash
docker-compose -f docker-compose.dev.yml up -d
```

### 2. Production Server
Use `docker-compose.yml` as-is, but:
- Change passwords
- Enable HTTPS
- Configure firewall
- Use reverse proxy (nginx/Caddy)

### 3. Cloud Platforms

#### Azure Container Apps
```bash
az containerapp up \
  --name restaurant-manager \
  --resource-group myResourceGroup \
  --location eastus \
  --environment myEnvironment \
  --image restaurant-manager:latest
```

#### AWS ECS
```bash
# Push to ECR
aws ecr get-login-password | docker login --username AWS --password-stdin <account>.dkr.ecr.region.amazonaws.com
docker tag restaurant-manager:latest <account>.dkr.ecr.region.amazonaws.com/restaurant-manager:latest
docker push <account>.dkr.ecr.region.amazonaws.com/restaurant-manager:latest
```

#### Google Cloud Run
```bash
gcloud run deploy restaurant-manager \
  --image restaurant-manager:latest \
  --platform managed \
  --region us-central1 \
  --allow-unauthenticated
```

---

## ?? Monitoring

### Docker Stats
```bash
docker stats restaurant-app restaurant-db
```

### Container Logs
```bash
# Export logs
docker-compose logs > logs.txt

# Watch logs
docker-compose logs -f --tail=100
```

---

## ?? Security Best Practices

1. **Change default passwords** immediately
2. **Use environment variables** for secrets
3. **Enable HTTPS** in production
4. **Use non-root user** in Dockerfile
5. **Scan images** for vulnerabilities:
   ```bash
   docker scan restaurant-manager:latest
   ```
6. **Limit container resources**:
   ```yaml
   deploy:
     resources:
       limits:
         cpus: '2'
         memory: 2G
   ```

---

## ?? Next Steps

1. ? Test locally: `docker-compose up -d`
2. ? Access: http://localhost:5000
3. ? Login with admin credentials
4. ? Test all features
5. ? Check logs: `docker-compose logs -f`
6. ? Update passwords before production
7. ? Deploy to your platform of choice

---

## ?? Support

If you encounter issues:

1. Check container status: `docker-compose ps`
2. View logs: `docker-compose logs -f`
3. Verify network: `docker network inspect restaurant-network`
4. Test health endpoint: `curl http://localhost:5000/health`
5. Restart services: `docker-compose restart`

---

## ?? Success Criteria

Your application is working if:
- ? `docker-compose ps` shows all containers as "Up"
- ? http://localhost:5000 loads the login page
- ? You can login with admin credentials
- ? Health endpoint returns `{"status":"healthy"}`
- ? Database persists data after restart

---

**Happy Deploying! ??**

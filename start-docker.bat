@echo off
REM Restaurant Manager - Docker Startup Script

echo.
echo ???  Restaurant Manager - Starting Docker Containers...
echo.

REM Check if Docker is running
docker info >nul 2>&1
if errorlevel 1 (
    echo ? Error: Docker is not running. Please start Docker Desktop.
    exit /b 1
)

REM Build and start containers
echo ?? Building Docker images...
docker-compose build

echo.
echo ?? Starting services...
docker-compose up -d

echo.
echo ? Waiting for services to be healthy...
timeout /t 10 /nobreak >nul

REM Check status
echo.
echo ?? Service Status:
docker-compose ps

echo.
echo ? Application is starting!
echo.
echo ?? Access the application at: http://localhost:5000
echo.
echo ?? Login Credentials:
echo    Admin:   admin@restaurant.com / admin123
echo    Cashier: cashier@restaurant.com / cashier123
echo.
echo ?? Useful commands:
echo    View logs:    docker-compose logs -f
echo    Stop:         docker-compose stop
echo    Restart:      docker-compose restart
echo    Remove all:   docker-compose down -v
echo.
pause

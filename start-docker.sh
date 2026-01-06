#!/bin/bash

# Restaurant Manager - Docker Startup Script

echo "???  Restaurant Manager - Starting Docker Containers..."
echo ""

# Check if Docker is running
if ! docker info > /dev/null 2>&1; then
    echo "? Error: Docker is not running. Please start Docker Desktop."
    exit 1
fi

# Check if docker-compose is available
if ! command -v docker-compose &> /dev/null; then
    echo "? Error: docker-compose is not installed."
    exit 1
fi

# Build and start containers
echo "?? Building Docker images..."
docker-compose build

echo ""
echo "?? Starting services..."
docker-compose up -d

echo ""
echo "? Waiting for SQL Server to initialize (this may take 1-2 minutes)..."
sleep 30

echo "? Waiting for services to be healthy..."
sleep 30

# Check status
echo ""
echo "?? Service Status:"
docker-compose ps

echo ""
echo "? Application is starting!"
echo ""
echo "?? Access the application at: http://localhost:5000"
echo ""
echo "?? Login Credentials:"
echo "   Admin:   admin@restaurant.com / admin123"
echo "   Cashier: cashier@restaurant.com / cashier123"
echo ""
echo "?? Useful commands:"
echo "   View logs:    docker-compose logs -f"
echo "   Stop:         docker-compose stop"
echo "   Restart:      docker-compose restart"
echo "   Remove all:   docker-compose down -v"
echo ""

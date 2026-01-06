echo "=========================================="
echo "Testing Complete!"
echo "=========================================="
echo ""

esac
        ;;
        exit 1
        echo -e "${RED}Invalid choice${NC}"
    *)
        ;;
        cleanup
    5)
        ;;
        show_logs
    4)
        ;;
        build
    3)
        ;;
        test_app
        sleep 5
        test_compose
        cleanup
    2)
        ;;
        test_app
        run
        build
        cleanup
    1)
case $choice in

read -p "Enter choice [1-5]: " choice
echo ""
echo "5) Cleanup and exit"
echo "4) View logs of running container"
echo "3) Just build (no run)"
echo "2) Build and run with Docker Compose (recommended)"
echo "1) Build and run with Docker"
echo "Select testing option:"
# Main menu

}
    fi
        echo "  docker logs restaurant-manager-test"
        echo "It may still be starting. Check logs with:"
        echo -e "${YELLOW}⚠ Application is not responding yet${NC}"
    else
        fi
            xdg-open http://localhost:8080 2>/dev/null || echo "Open http://localhost:8080 in your browser"
        elif [[ "$OSTYPE" == "linux-gnu"* ]]; then
            open http://localhost:8080
        if [[ "$OSTYPE" == "darwin"* ]]; then
        echo "Opening in browser..."
        echo ""
        echo -e "${GREEN}✓ Application is responding${NC}"
    if curl -f -s http://localhost:8080 > /dev/null; then
    
    sleep 10
    # Wait a bit for app to fully start
    
    echo ""
    echo "Testing application endpoint..."
test_app() {
# Function to test the application

}
    docker logs -f restaurant-manager-test 2>/dev/null || docker-compose logs -f
    echo ""
    echo "Showing container logs (Ctrl+C to exit)..."
show_logs() {
# Function to show logs

}
    fi
        exit 1
        echo -e "${RED}✗ Docker Compose failed${NC}"
        echo ""
    else
        echo "  docker-compose down"
        echo "Stop with:"
        echo ""
        echo "  docker-compose logs -f"
        echo "View logs with:"
        echo ""
        echo "  http://localhost:8080"
        echo "Application is available at:"
        echo ""
        echo -e "${GREEN}✓ Docker Compose deployment successful${NC}"
        echo ""
    if docker-compose up -d --build; then
    
    echo ""
    echo "Testing with Docker Compose..."
test_compose() {
# Function to test with docker-compose

}
    fi
        exit 1
        docker logs restaurant-manager-test
        echo "Check logs:"
        echo -e "${RED}✗ Container failed to start${NC}"
    else
        echo "  docker rm restaurant-manager-test"
        echo "  docker stop restaurant-manager-test"
        echo "Stop container with:"
        echo ""
        echo "  docker logs -f restaurant-manager-test"
        echo "View logs with:"
        echo ""
        echo "  http://localhost:8080"
        echo "Application is available at:"
        echo ""
        echo -e "${GREEN}✓ Container is running${NC}"
    if docker ps | grep -q restaurant-manager-test; then
    # Check if container is running
    
    sleep 5
    echo "Waiting for application to start..."
    echo ""
    echo -e "${GREEN}✓ Container started${NC}"
    
        restaurant-manager:test
        --env-file .env \
        -p 8080:8080 \
        --name restaurant-manager-test \
    docker run -d \
    
    echo ""
    echo "Starting container..."
run() {
# Function to run the container

}
    fi
        exit 1
        echo -e "${RED}✗ Docker build failed${NC}"
        echo ""
    else
        echo ""
        echo "Image size: $SIZE"
        SIZE=$(docker images restaurant-manager:test --format "{{.Size}}")
        # Show image size
        
        echo -e "${GREEN}✓ Docker image built successfully${NC}"
        echo ""
    if docker build -t restaurant-manager:test .; then
    
    echo ""
    echo "This may take a few minutes..."
    echo "Building Docker image..."
build() {
# Function to build the Docker image

}
    echo ""
    echo -e "${GREEN}✓ Cleanup complete${NC}"
    docker rm -f restaurant-manager 2>/dev/null || true
    docker-compose down 2>/dev/null || true
    echo "Cleaning up previous containers..."
cleanup() {
# Function to clean up previous containers

fi
    echo ""
    echo -e "${GREEN}✓ .env file created${NC}"
    cp .env.example .env
    echo "Creating .env from .env.example..."
    echo -e "${YELLOW}⚠ .env file not found${NC}"
if [ ! -f .env ]; then
# Check if .env file exists

echo ""
echo -e "${GREEN}✓ Docker is running${NC}"

fi
    exit 1
    echo "Please start Docker Desktop and try again"
    echo -e "${RED}✗ Docker is not running${NC}"
if ! docker info > /dev/null 2>&1; then
# Check if Docker is running

NC='\033[0m' # No Color
YELLOW='\033[1;33m'
GREEN='\033[0;32m'
RED='\033[0;31m'
# Colors for output

echo ""
echo "=========================================="
echo "Restaurant Manager - Local Docker Testing"
echo "=========================================="

set -e

# This script helps you test the Docker build locally before deploying to VPS
# Local Docker Testing Script



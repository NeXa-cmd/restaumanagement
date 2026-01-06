#!/bin/bash

# VPS Deployment Script for Restaurant Manager
# This script helps you deploy the application to your VPS

set -e

echo "=========================================="
echo "Restaurant Manager - VPS Deployment Script"
echo "=========================================="
echo ""

# Configuration
VPS_USER="${VPS_USER:-root}"
VPS_HOST="${VPS_HOST}"
DEPLOY_PATH="${DEPLOY_PATH:-/opt/restaurant-manager}"

# Check if VPS_HOST is set
if [ -z "$VPS_HOST" ]; then
    echo "Please enter your VPS IP address or hostname:"
    read -r VPS_HOST
fi

echo "VPS Host: $VPS_HOST"
echo "VPS User: $VPS_USER"
echo "Deploy Path: $DEPLOY_PATH"
echo ""

# Function to check SSH connection
check_ssh() {
    echo "Testing SSH connection..."
    if ssh -o ConnectTimeout=10 "$VPS_USER@$VPS_HOST" "echo 'SSH connection successful'" &> /dev/null; then
        echo "✓ SSH connection successful"
        return 0
    else
        echo "✗ SSH connection failed"
        echo "Please ensure:"
        echo "  1. SSH is enabled on your VPS"
        echo "  2. You have SSH key authentication set up"
        echo "  3. The hostname/IP is correct"
        exit 1
    fi
}

# Function to install Docker on VPS
install_docker() {
    echo ""
    echo "Installing Docker on VPS..."
    ssh "$VPS_USER@$VPS_HOST" 'bash -s' << 'ENDSSH'
        # Check if Docker is already installed
        if command -v docker &> /dev/null; then
            echo "Docker is already installed"
            docker --version
        else
            echo "Installing Docker..."
            curl -fsSL https://get.docker.com -o get-docker.sh
            sh get-docker.sh
            rm get-docker.sh
            
            # Start Docker service
            systemctl start docker
            systemctl enable docker
            
            echo "✓ Docker installed successfully"
            docker --version
        fi
        
        # Install Docker Compose
        if command -v docker-compose &> /dev/null; then
            echo "Docker Compose is already installed"
            docker-compose --version
        else
            echo "Installing Docker Compose..."
            curl -L "https://github.com/docker/compose/releases/latest/download/docker-compose-$(uname -s)-$(uname -m)" -o /usr/local/bin/docker-compose
            chmod +x /usr/local/bin/docker-compose
            echo "✓ Docker Compose installed successfully"
            docker-compose --version
        fi
ENDSSH
}

# Function to deploy application
deploy_app() {
    echo ""
    echo "Building Docker image locally..."
    docker build -t restaurant-manager:latest .
    
    echo ""
    echo "Saving Docker image..."
    docker save restaurant-manager:latest | gzip > /tmp/restaurant-manager.tar.gz
    
    echo ""
    echo "Creating deployment directory on VPS..."
    ssh "$VPS_USER@$VPS_HOST" "mkdir -p $DEPLOY_PATH"
    
    echo ""
    echo "Copying files to VPS..."
    scp /tmp/restaurant-manager.tar.gz "$VPS_USER@$VPS_HOST:$DEPLOY_PATH/"
    scp docker-compose.yml "$VPS_USER@$VPS_HOST:$DEPLOY_PATH/"
    scp .env.example "$VPS_USER@$VPS_HOST:$DEPLOY_PATH/"
    
    echo ""
    echo "Deploying on VPS..."
    ssh "$VPS_USER@$VPS_HOST" "bash -s" << ENDSSH
        cd $DEPLOY_PATH
        
        # Load Docker image
        echo "Loading Docker image..."
        docker load < restaurant-manager.tar.gz
        
        # Create .env file if it doesn't exist
        if [ ! -f .env ]; then
            echo "Creating .env file from example..."
            cp .env.example .env
            echo "⚠ Please edit $DEPLOY_PATH/.env with your actual configuration"
        fi
        
        # Stop and remove old containers
        echo "Stopping old containers..."
        docker-compose down 2>/dev/null || true
        
        # Start new containers
        echo "Starting new containers..."
        docker-compose up -d
        
        # Clean up
        echo "Cleaning up..."
        rm restaurant-manager.tar.gz
        docker system prune -f
        
        echo ""
        echo "✓ Deployment completed successfully!"
        echo ""
        echo "Application is running at: http://$VPS_HOST:8080"
        echo ""
        echo "To view logs:"
        echo "  docker-compose -f $DEPLOY_PATH/docker-compose.yml logs -f"
ENDSSH
    
    # Clean up local temp file
    rm /tmp/restaurant-manager.tar.gz
}

# Main script
main() {
    check_ssh
    
    echo ""
    echo "Do you want to install/update Docker on the VPS? (y/n)"
    read -r install_docker_choice
    
    if [ "$install_docker_choice" = "y" ] || [ "$install_docker_choice" = "Y" ]; then
        install_docker
    fi
    
    echo ""
    echo "Ready to deploy. Continue? (y/n)"
    read -r deploy_choice
    
    if [ "$deploy_choice" = "y" ] || [ "$deploy_choice" = "Y" ]; then
        deploy_app
        echo ""
        echo "=========================================="
        echo "Deployment Complete!"
        echo "=========================================="
        echo ""
        echo "Your application should now be running at:"
        echo "  http://$VPS_HOST:8080"
        echo ""
        echo "Next steps:"
        echo "  1. Configure your domain DNS to point to $VPS_HOST"
        echo "  2. Set up SSL/HTTPS with a reverse proxy (Nginx/Caddy)"
        echo "  3. Configure firewall rules"
        echo "  4. Monitor application logs"
        echo ""
    else
        echo "Deployment cancelled."
        exit 0
    fi
}

main


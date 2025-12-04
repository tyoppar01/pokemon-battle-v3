#!/bin/bash

echo "======================================"
echo "Pokemon Battle Backend Setup & Run"
echo "======================================"
echo ""

# Navigate to backend directory
cd "$(dirname "$0")"

# Check if .env file exists
if [ ! -f ".env" ]; then
    echo "⚠ .env file not found!"
    echo "Creating .env file with default configuration..."
    cat > .env << 'EOF'
# Database Configuration
DATABASE_PATH=Databases/pokemon_battle.db

# Server Configuration
API_PORT=5000
ASPNETCORE_ENVIRONMENT=Development

# CORS Configuration - Allowed Origins (comma-separated)
CORS_ORIGINS=http://localhost:8000,http://127.0.0.1:8000,http://localhost:5500,http://127.0.0.1:5500,http://localhost:5050,http://127.0.0.1:5050,http://localhost:8080,http://127.0.0.1:8080
EOF
    echo "✓ Created .env file with default settings"
    echo ""
else
    echo "✓ .env file found"
fi

# Check if database exists, if not it will be created on startup
if [ -f "pokemonbattle.db" ]; then
    echo "✓ Database file found: pokemonbattle.db"
else
    echo "⚠ Database file not found - will be created on startup"
fi

echo ""
echo "Building the project..."
dotnet build

if [ $? -eq 0 ]; then
    echo ""
    echo "✓ Build successful!"
    echo ""
    echo "Starting the server..."
    echo "======================================"
    dotnet run
else
    echo ""
    echo "✗ Build failed. Please check the errors above."
    exit 1
fi

#!/bin/bash

echo "======================================"
echo "Pokemon Battle Backend Setup & Run"
echo "======================================"
echo ""

# Navigate to backend directory
cd "$(dirname "$0")"

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

#!/bin/bash

# Pokemon Battle V3 - Startup Script
# Starts both backend and frontend servers

echo "🎮 Starting Pokemon Battle V3..."

# Start backend (C# .NET)
echo "🔧 Starting Backend API (port 5000)..."
cd backend
chmod +x run.sh
./run.sh &
BACKEND_PID=$!
cd ..

# Wait for backend to start
sleep 5

# Start frontend (Python HTTP server)
echo "🌐 Starting Frontend Server (port 8000)..."
cd frontend
python3 -m http.server 8000 &
FRONTEND_PID=$!
cd ..

echo ""
echo "Servers Started..."
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo "Backend:  http://localhost:5000"
echo "Frontend:  http://localhost:8000"
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo ""
echo "Press Ctrl+C to stop all servers..."

# Wait for user interrupt
trap "echo ''; echo '🛑 Stopping servers...'; kill $BACKEND_PID $FRONTEND_PID; exit" INT
wait

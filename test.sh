#!/bin/bash

echo "Running backend tests..."
cd backend && dotnet test
BACKEND_EXIT=$?

echo ""
echo "Running frontend tests..."
cd ../frontend && npm test

FRONTEND_EXIT=$?

if [ $BACKEND_EXIT -ne 0 ] || [ $FRONTEND_EXIT -ne 0 ]; then
    exit 1
fi

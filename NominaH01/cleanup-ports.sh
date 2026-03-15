#!/bin/bash
# Quick Port Cleanup Script for .NET Development
# Usage: ./cleanup-ports.sh [port_number]

PORT=${1:-7094}
echo "Checking port $PORT..."

# Check if port is in use
if lsof -i:$PORT >/dev/null 2>&1; then
    echo "Port $PORT is in use. Process details:"
    lsof -i:$PORT
    
    echo -e "\nAttempting to free port $PORT..."
    # Kill processes using the port
    lsof -ti:$PORT | xargs kill -9 2>/dev/null
    
    # Wait a moment for the port to be released
    sleep 2
    
    # Check again
    if lsof -i:$PORT >/dev/null 2>&1; then
        echo "WARNING: Port $PORT is still in use after cleanup attempt"
        echo "You may need to restart VS Code or your computer"
        lsof -i:$PORT
        exit 1
    else
        echo "SUCCESS: Port $PORT has been freed"
    fi
else
    echo "Port $PORT is available"
fi

# Also check for any dotnet processes that might be hanging
echo -e "\nChecking for hanging dotnet processes..."
DOTNET_PIDS=$(pgrep -f "2FA.dll" 2>/dev/null || true)
if [ ! -z "$DOTNET_PIDS" ]; then
    echo "Found hanging dotnet processes: $DOTNET_PIDS"
    echo "Killing these processes..."
    echo $DOTNET_PIDS | xargs kill -9 2>/dev/null || true
    echo "Hanging processes killed"
else
    echo "No hanging dotnet processes found"
fi

echo -e "\nPort cleanup completed. You can now start your application."
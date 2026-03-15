#!/usr/bin/env bash
set -euo pipefail

# Script to initialize dotnet user-secrets for the 2FA project
# Run this from the repository root

PROJECT_DIR="2FA"

if [ ! -d "$PROJECT_DIR" ]; then
  echo "Project directory $PROJECT_DIR not found. Run from repo root." >&2
  exit 1
fi

pushd "$PROJECT_DIR" >/dev/null

echo "Initializing user-secrets for project in $PROJECT_DIR"
# Initialize user-secrets (creates a userSecretsId in the .csproj)
dotnet user-secrets init || true

echo "Setting placeholder connection string (replace with secure value)"
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=YOUR_SERVER;Database=2FA;User Id=app_user;Password=YOUR_PASSWORD;"

echo "Done. For development, export the same variable as env if preferred:" 
echo "export ConnectionStrings__DefaultConnection=\"Server=YOUR_SERVER;Database=2FA;User Id=app_user;Password=YOUR_PASSWORD;\""

popd >/dev/null

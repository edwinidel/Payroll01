#!/usr/bin/env bash
set -euo pipefail

echo "Purge secrets helper for this repository"
echo
echo "IMPORTANT: Read this script before running. It will rewrite Git history."

REPO_DIR=$(git rev-parse --show-toplevel 2>/dev/null || true)
if [ -z "$REPO_DIR" ]; then
  echo "Error: run this from inside a git repository." >&2
  exit 1
fi

echo "Repository: $REPO_DIR"

echo "Step 1: Create a local mirror backup (safe copy)"
echo "  git clone --mirror $REPO_DIR ../repo-backup.git"
echo
echo "Step 2: Choose a tool to clean history: git-filter-repo (recommended) or BFG"
echo
echo "If you have git-filter-repo installed, you can run (example):"
echo "  git clone --mirror $REPO_DIR repo.git"
echo "  cd repo.git"
echo "  git filter-repo --replace-text ../scripts/strings_to_replace.txt"
echo "  git push --force"
echo
echo "Or using BFG (Java):"
echo "  git clone --mirror $REPO_DIR repo.git"
echo "  java -jar bfg.jar --replace-text ../scripts/strings_to_replace.txt repo.git"
echo "  cd repo.git"
echo "  git reflog expire --expire=now --all && git gc --prune=now --aggressive"
echo "  git push --force"
echo
echo "strings_to_replace.txt contains the plaintext tokens we want to replace."
echo "Open and review scripts/strings_to_replace.txt and replace placeholders if needed."

cat <<'EOF'
Example strings_to_replace.txt format (this repo includes a sample at scripts/strings_to_replace.txt):
Yanaris1624*==>[REDACTED_PASSWORD]
Server=192.168.0.12==>[REDACTED_SERVER]
Server=192.168.0.20==>[REDACTED_SERVER]
Server=192.168.0.71==>[REDACTED_SERVER]
EOF

echo
echo "After cleaning, inform all collaborators to re-clone the repository."
echo "See ROTATE_DB_CREDENTIALS.md for guidance on rotating DB credentials and creating low-privilege accounts."

exit 0

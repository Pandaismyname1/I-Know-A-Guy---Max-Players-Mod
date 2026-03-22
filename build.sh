#!/bin/bash
set -e

SCRIPT_DIR="$(cd "$(dirname "$0")" && pwd)"
cd "$SCRIPT_DIR"

echo "=== Building IKnowAGuyMaxPlayersMod ==="
dotnet build -c Release

DLL="bin/Release/net6.0/IKnowAGuyMaxPlayersMod.dll"
if [ ! -f "$DLL" ]; then
    echo "ERROR: Build output not found at $DLL"
    exit 1
fi

# Deploy to Mods/
cp "$DLL" ../Mods/
echo "Deployed to ../Mods/"

# Create Thunderstore package
echo "=== Creating Thunderstore package ==="
DIST="dist"
rm -rf "$DIST"
mkdir -p "$DIST/Mods"

cp thunderstore/manifest.json "$DIST/"
cp thunderstore/README.md "$DIST/"
cp "$DLL" "$DIST/Mods/"

# Check for icon
if [ -f thunderstore/icon.png ]; then
    cp thunderstore/icon.png "$DIST/"
else
    echo "WARNING: thunderstore/icon.png not found - you'll need to add a 256x256 PNG before uploading"
fi

cd "$DIST"
zip -r ../IKnowAGuyMaxPlayersMod-thunderstore.zip .
cd ..

echo "=== Done ==="
echo "  Mod DLL:          $DLL"
echo "  Deployed to:      ../Mods/IKnowAGuyMaxPlayersMod.dll"
echo "  Thunderstore zip: IKnowAGuyMaxPlayersMod-thunderstore.zip"

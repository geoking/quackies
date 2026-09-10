#!/usr/bin/env bash
set -euo pipefail

# Compile the independent rules library before replacing Unity's checked-in copy.
# The destination .meta is preserved, so scene and assembly references remain valid.
quackies_root="$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")/.." && pwd)"
dotnet build "$quackies_root/src/Quackies.Core/Quackies.Core.csproj" --configuration Release
cp "$quackies_root/src/Quackies.Core/bin/Release/netstandard2.1/Quackies.Core.dll" \
   "$quackies_root/unity/Quackies.Unity/Assets/Plugins/Quackies.Core.dll"
printf '%s\n' "Core copied to Unity. Let the Editor finish importing before starting Play Mode."

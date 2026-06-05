#!/usr/bin/env bash
set -euo pipefail

cd /home/rhy/Projects/Civ2-clone

dotnet build Civ2clone.sln
dotnet test Civ2clone.sln

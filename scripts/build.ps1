$ErrorActionPreference = "Stop"

dotnet publish src/TextForge.Desktop/TextForge.Desktop.csproj -c Release -r win-x64 --self-contained true -o ./dist

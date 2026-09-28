$ErrorActionPreference = "Stop"

$distExe = Join-Path $PSScriptRoot "..\dist\TextForge.Desktop.exe"

if (-not (Test-Path $distExe)) {
    & "$PSScriptRoot\build.ps1"
}

& $distExe

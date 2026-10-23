# build.ps1 - Build HandOfFate2Access and deploy the plugin, its Core library,
# the x86 Tolk runtime and the sounds into the game's BepInEx\plugins folder.

param(
    [switch]$NoBuild,
    [switch]$Help
)

if ($Help) {
    Write-Host "Usage: .\build.ps1 [-NoBuild] [-Help]"
    Write-Host "  -NoBuild  Skip building; just redeploy the last built DLLs and native deps"
    exit 0
}

$ErrorActionPreference = "Stop"
. "$PSScriptRoot\tools\Find-Game.ps1"
$env:HOF2_GAME = $Game

if (-not (Test-Path "$Game\BepInEx\core\BepInEx.dll")) {
    Write-Host "ERROR: BepInEx is not installed at $Game\BepInEx. Run setup-bepinex.ps1 first." -ForegroundColor Red
    exit 1
}

$ProjectDir  = "$PSScriptRoot\HandOfFate2Access"
$BuildDir    = "$ProjectDir\bin\Release"
$PluginsDir  = "$Game\BepInEx\plugins\HandOfFate2Access"

if (-not $NoBuild) {
    Write-Host "Building HandOfFate2Access (game: $Game)..." -ForegroundColor Cyan
    dotnet build "$ProjectDir\HandOfFate2Access.csproj" -c Release
    if ($LASTEXITCODE -ne 0) {
        Write-Host "Build FAILED." -ForegroundColor Red
        exit 1
    }
}

New-Item -ItemType Directory -Path $PluginsDir -Force | Out-Null
foreach ($name in @("HandOfFate2Access.dll", "HandOfFate2Access.Core.dll")) {
    if (-not (Test-Path "$BuildDir\$name")) {
        Write-Host "ERROR: $name not found in $BuildDir" -ForegroundColor Red
        exit 1
    }
    Copy-Item "$BuildDir\$name" "$PluginsDir\$name" -Force
}
Write-Host "Deployed HandOfFate2Access.dll + Core to $PluginsDir" -ForegroundColor Green

$TolkFiles = Get-ChildItem -Path "$PSScriptRoot\third_party\tolk\dist\x86" -Include *.dll, *.ini -File -Recurse
foreach ($f in $TolkFiles) { Copy-Item $f.FullName "$PluginsDir\$($f.Name)" -Force }
Write-Host "Deployed $($TolkFiles.Count) Tolk runtime file(s)" -ForegroundColor Green

Copy-Item "$PSScriptRoot\native\hofsapi\HofSapi.dll" "$PluginsDir\HofSapi.dll" -Force
Write-Host "Deployed HofSapi.dll" -ForegroundColor Green

$SoundsSrc = "$PSScriptRoot\sounds"
if (Test-Path $SoundsSrc) {
    $SoundsDst = "$PluginsDir\sounds"
    New-Item -ItemType Directory -Path $SoundsDst -Force | Out-Null
    $SoundFiles = Get-ChildItem -Path $SoundsSrc -Filter *.wav -File
    foreach ($f in $SoundFiles) { Copy-Item $f.FullName "$SoundsDst\$($f.Name)" -Force }
    Write-Host "Deployed $($SoundFiles.Count) sound file(s)" -ForegroundColor Green
}

Write-Host "Done. Launch the game and listen for the startup line." -ForegroundColor Cyan

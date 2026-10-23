# release.ps1 - Assemble the player release zip. The zip extracts directly into the
# game folder (the one with "Hand of Fate 2.exe") and contains everything the mod
# needs: the BepInEx loader (x86, default Application entrypoint), the mod DLLs, the
# Tolk and SAPI native runtimes, and the sound files, laid out exactly as
# setup-bepinex.ps1 + build.ps1 deploy them. The layout is staged fresh each run from
# the repo's canonical sources, so it cannot drift from a developer install. The staged
# payload (release\payload) is committed: CI cannot build the plugin (it references the
# game's own DLLs), so the release workflow zips the committed payload.

param(
    [switch]$NoBuild,
    [switch]$SkipTests,
    [switch]$Help
)

if ($Help) {
    Write-Host "Usage: .\release.ps1 [-NoBuild] [-SkipTests] [-Help]"
    Write-Host "  -NoBuild    Skip building; package the last built DLLs"
    Write-Host "  -SkipTests  Skip the offline test suite"
    Write-Host "Output: release\HandOfFate2Access-v<version>.zip"
    exit 0
}

$ErrorActionPreference = "Stop"

$props = Get-Content "$PSScriptRoot\Directory.Build.props" -Raw
if ($props -notmatch '<Version>([^<]+)</Version>') {
    Write-Host "ERROR: no <Version> in Directory.Build.props" -ForegroundColor Red
    exit 1
}
$Version = $Matches[1]

if (-not $NoBuild) {
    . "$PSScriptRoot\tools\Find-Game.ps1"
    $env:HOF2_GAME = $Game
    Write-Host "Building HandOfFate2Access v$Version (Release)..." -ForegroundColor Cyan
    dotnet build "$PSScriptRoot\HandOfFate2Access\HandOfFate2Access.csproj" -c Release
    if ($LASTEXITCODE -ne 0) {
        Write-Host "Build FAILED." -ForegroundColor Red
        exit 1
    }
}
if (-not $SkipTests) {
    Write-Host "Running the offline test suite..." -ForegroundColor Cyan
    dotnet test "$PSScriptRoot\HandOfFate2Access.Tests\HandOfFate2Access.Tests.csproj"
    if ($LASTEXITCODE -ne 0) {
        Write-Host "Tests FAILED; not packaging." -ForegroundColor Red
        exit 1
    }
}

$ReleaseDir = "$PSScriptRoot\release"
$Stage      = "$ReleaseDir\payload"
if (Test-Path $Stage) { Remove-Item $Stage -Recurse -Force }
$PluginsDir = "$Stage\BepInEx\plugins\HandOfFate2Access"
New-Item -ItemType Directory -Path $PluginsDir -Force | Out-Null

# BepInEx loader: doorstop proxy at the game root, core, and the config seed.
$Vendor = "$PSScriptRoot\third_party\bepinex"
foreach ($f in @("winhttp.dll", "doorstop_config.ini")) { Copy-Item "$Vendor\$f" "$Stage\$f" }
if (Test-Path "$Vendor\.doorstop_version") { Copy-Item "$Vendor\.doorstop_version" "$Stage\.doorstop_version" }
New-Item -ItemType Directory -Path "$Stage\BepInEx\core", "$Stage\BepInEx\config" -Force | Out-Null
Copy-Item "$Vendor\BepInEx\core\*" "$Stage\BepInEx\core\"
Copy-Item "$Vendor\BepInEx\config\BepInEx.cfg" "$Stage\BepInEx\config\BepInEx.cfg"

$BuildDir = "$PSScriptRoot\HandOfFate2Access\bin\Release"
foreach ($name in @("HandOfFate2Access.dll", "HandOfFate2Access.Core.dll")) {
    Copy-Item "$BuildDir\$name" "$PluginsDir\$name"
}
Get-ChildItem "$PSScriptRoot\third_party\tolk\dist\x86" -Include *.dll, *.ini -File -Recurse |
    Copy-Item -Destination $PluginsDir
Copy-Item "$PSScriptRoot\native\hofsapi\HofSapi.dll" "$PluginsDir\HofSapi.dll"
New-Item -ItemType Directory -Path "$PluginsDir\sounds" -Force | Out-Null
Copy-Item "$PSScriptRoot\sounds\*.wav" "$PluginsDir\sounds\"

# The player README in every game language, as Markdown and HTML, at the zip root.
& "$PSScriptRoot\tools\Build-Readme.ps1"
if ($LASTEXITCODE -ne 0) { Write-Host "README rendering FAILED; not packaging." -ForegroundColor Red; exit 1 }
New-Item -ItemType Directory -Path "$Stage\HandOfFate2Access-README" -Force | Out-Null
Copy-Item "$PSScriptRoot\README.md" "$Stage\HandOfFate2Access-README\README.en.md"
Copy-Item "$PSScriptRoot\docs\README.*.md" "$Stage\HandOfFate2Access-README\"
Copy-Item "$PSScriptRoot\docs\html\README.*.html" "$Stage\HandOfFate2Access-README\"

# The playtest remote must never ship.
Get-ChildItem $Stage -Recurse -Filter "playtest-remote*" | Remove-Item -Force

$required = @(
    "winhttp.dll",
    "doorstop_config.ini",
    "BepInEx\core\BepInEx.dll",
    "BepInEx\core\BepInEx.Preloader.dll",
    "BepInEx\core\0Harmony.dll",
    "BepInEx\config\BepInEx.cfg",
    "BepInEx\plugins\HandOfFate2Access\HandOfFate2Access.dll",
    "BepInEx\plugins\HandOfFate2Access\HandOfFate2Access.Core.dll",
    "BepInEx\plugins\HandOfFate2Access\Tolk.dll",
    "BepInEx\plugins\HandOfFate2Access\HofSapi.dll",
    "BepInEx\plugins\HandOfFate2Access\sounds\walltone_collision.wav"
)
$missing = $required | Where-Object { -not (Test-Path "$Stage\$_") }
if ($missing) {
    Write-Host "ERROR: staged release is missing:" -ForegroundColor Red
    $missing | ForEach-Object { Write-Host "  $_" -ForegroundColor Red }
    exit 1
}

$Zip = "$ReleaseDir\HandOfFate2Access-v$Version.zip"
if (Test-Path $Zip) { Remove-Item $Zip -Force }
Add-Type -AssemblyName System.IO.Compression.FileSystem
[System.IO.Compression.ZipFile]::CreateFromDirectory($Stage, $Zip)

$size = "{0:N1}" -f ((Get-Item $Zip).Length / 1MB)
Write-Host ""
Write-Host "Release ready: $Zip ($size MB)" -ForegroundColor Green
Write-Host "Players extract it directly into the game folder and play." -ForegroundColor Cyan

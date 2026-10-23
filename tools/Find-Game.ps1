# Find-Game.ps1 - dot-sourced by the repo scripts. Resolves the Hand of Fate 2
# install folder into $Game: the HOF2_GAME env var wins; otherwise every Steam
# library listed in libraryfolders.vdf is searched; otherwise the default path.

function Find-HoF2Game {
    if ($env:HOF2_GAME) { return $env:HOF2_GAME }
    $roots = @()
    foreach ($key in @("HKCU:\Software\Valve\Steam", "HKLM:\SOFTWARE\WOW6432Node\Valve\Steam")) {
        $props = Get-ItemProperty -Path $key -ErrorAction SilentlyContinue
        if ($props.SteamPath) { $roots += $props.SteamPath }
        if ($props.InstallPath) { $roots += $props.InstallPath }
    }
    $roots += "C:\Program Files (x86)\Steam"
    $libraries = @()
    foreach ($root in $roots) {
        if (-not (Test-Path "$root\steamapps")) { continue }
        $libraries += $root
        $vdf = "$root\steamapps\libraryfolders.vdf"
        if (Test-Path $vdf) {
            [regex]::Matches((Get-Content $vdf -Raw), '"path"\s+"([^"]+)"') | ForEach-Object {
                $libraries += ($_.Groups[1].Value -replace '\\', '\')
            }
        }
    }
    foreach ($lib in ($libraries | Select-Object -Unique)) {
        $candidate = "$lib\steamapps\common\Hand of Fate 2"
        if (Test-Path "$candidate\Hand of Fate 2.exe") { return $candidate }
    }
    return "C:\Program Files (x86)\Steam\steamapps\common\Hand of Fate 2"
}

$Game = Find-HoF2Game
if (-not (Test-Path "$Game\Hand of Fate 2.exe")) {
    Write-Host "ERROR: Hand of Fate 2 not found at: $Game" -ForegroundColor Red
    Write-Host "Set the HOF2_GAME environment variable to the game folder." -ForegroundColor Red
    exit 1
}

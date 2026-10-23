# setup-bepinex.ps1 - Install the vendored BepInEx (third_party/bepinex) into the
# Hand of Fate 2 folder and guarantee the Application entrypoint. Idempotent:
# safe to re-run. After this, run build.ps1 to deploy the mod plugin.

$ErrorActionPreference = "Stop"
. "$PSScriptRoot\tools\Find-Game.ps1"

$Vendor = "$PSScriptRoot\third_party\bepinex"
if (-not (Test-Path "$Vendor\winhttp.dll")) {
    Write-Host "ERROR: vendored BepInEx not found at $Vendor" -ForegroundColor Red
    exit 1
}

Write-Host "Installing BepInEx into $Game ..." -ForegroundColor Cyan

foreach ($f in @("winhttp.dll", "doorstop_config.ini", "changelog.txt", ".doorstop_version")) {
    if (Test-Path "$Vendor\$f") { Copy-Item "$Vendor\$f" "$Game\$f" -Force }
}
$CoreDest = "$Game\BepInEx\core"
New-Item -ItemType Directory -Path $CoreDest -Force | Out-Null
Copy-Item "$Vendor\BepInEx\core\*" $CoreDest -Force
New-Item -ItemType Directory -Path "$Game\BepInEx\plugins" -Force | Out-Null

$CfgDir  = "$Game\BepInEx\config"
$CfgPath = "$CfgDir\BepInEx.cfg"
if (-not (Test-Path $CfgPath)) {
    New-Item -ItemType Directory -Path $CfgDir -Force | Out-Null
    Copy-Item "$Vendor\BepInEx\config\BepInEx.cfg" $CfgPath -Force
    Write-Host "Seeded BepInEx.cfg with the Application entrypoint." -ForegroundColor Green
} else {
    $cfg = Get-Content $CfgPath -Raw
    if ($cfg -match '(?ms)\[Preloader\.Entrypoint\].*?^\s*Type\s*=') {
        $cfg = [regex]::Replace($cfg, '(?ms)(\[Preloader\.Entrypoint\].*?^\s*Assembly\s*=\s*)\S+', '${1}UnityEngine.CoreModule.dll', 1)
        $cfg = [regex]::Replace($cfg, '(?ms)(\[Preloader\.Entrypoint\].*?^\s*Type\s*=\s*)\S+', '${1}Application', 1)
        [System.IO.File]::WriteAllText($CfgPath, $cfg)
        Write-Host "Patched existing BepInEx.cfg entrypoint to Application." -ForegroundColor Green
    } else {
        Add-Content $CfgPath "`r`n[Preloader.Entrypoint]`r`nAssembly = UnityEngine.CoreModule.dll`r`nType = Application`r`nMethod = .cctor`r`n"
        Write-Host "Appended the Application entrypoint to BepInEx.cfg." -ForegroundColor Green
    }
}

Write-Host ""
Write-Host "BepInEx installed. Now run build.ps1 to deploy the mod." -ForegroundColor Cyan

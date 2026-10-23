# Vendored BepInEx

BepInEx **5.4.23.2**, **win x86** build, the loader this mod targets.

- Source: https://github.com/BepInEx/BepInEx/releases/tag/v5.4.23.2
  (asset `BepInEx_win_x86_5.4.23.2.zip`)
- Architecture: **x86** is required. Hand of Fate 2 is 32-bit (Unity 2017.4.40f1
  Mono); the x64 build's `winhttp.dll` proxy will not inject.

## What's here

The extracted distribution (`BepInEx/core`, `winhttp.dll`, `doorstop_config.ini`)
plus one file the upstream zip does NOT ship:

- `BepInEx/config/BepInEx.cfg`: a seed config that pre-sets
  `[Preloader.Entrypoint] Assembly = UnityEngine.CoreModule.dll`,
  `Type = Application` (the default, verified on this game). See CLAUDE.md.

## Installing into the game

Run `setup-bepinex.ps1` at the repo root: it copies this distribution into the
game folder and guarantees the entrypoint (patching an existing `BepInEx.cfg` in
place if one is already there). Then run `build.ps1` to deploy the mod plugin.

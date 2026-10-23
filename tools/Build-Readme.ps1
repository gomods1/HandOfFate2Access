# Build-Readme.ps1 - Render the player README in every game language to HTML.
# The sources are docs\README.<lang>.md (the root README.md is the English one);
# each becomes docs\html\README.<lang>.html, a standalone page with its lang
# attribute set so a screen reader switches to the right voice. Needs pandoc.
$ErrorActionPreference = "Stop"
$Root = Split-Path $PSScriptRoot -Parent
$Out = "$Root\docs\html"
New-Item -ItemType Directory -Path $Out -Force | Out-Null
if (-not (Get-Command pandoc -ErrorAction SilentlyContinue)) {
    Write-Host "ERROR: pandoc not found (https://pandoc.org)" -ForegroundColor Red
    exit 1
}
$sources = @(@{ Path = "$Root\README.md"; Lang = "en" })
foreach ($f in Get-ChildItem "$Root\docs" -Filter "README.*.md" -File) {
    $lang = $f.BaseName -replace '^README\.', ''
    $sources += @{ Path = $f.FullName; Lang = $lang }
}
foreach ($s in $sources) {
    $html = "$Out\README.$($s.Lang).html"
    pandoc $s.Path -f gfm -t html5 -s --metadata "lang=$($s.Lang)" --metadata "pagetitle=Hand of Fate 2 Access" -o $html
    if ($LASTEXITCODE -ne 0) { Write-Host "ERROR: pandoc failed on $($s.Path)" -ForegroundColor Red; exit 1 }
    Write-Host "  $html"
}
Write-Host "Rendered $($sources.Count) README page(s) to $Out" -ForegroundColor Green

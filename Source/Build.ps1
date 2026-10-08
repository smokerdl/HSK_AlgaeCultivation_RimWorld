$ErrorActionPreference = 'Stop'

$sourceDir = $PSScriptRoot
$modDir = Split-Path -Parent $sourceDir
$defaultRimWorld = Split-Path -Parent (Split-Path -Parent $modDir)

if ($env:RIMWORLD_DIR) {
    $rimWorldDir = $env:RIMWORLD_DIR
} else {
    $rimWorldDir = $defaultRimWorld
}

$rimWorldDir = $rimWorldDir.TrimEnd('\', '/')

$assemblyPath = Join-Path $rimWorldDir 'RimWorldWin64_Data\Managed\Assembly-CSharp.dll'
if (-not (Test-Path $assemblyPath)) {
    throw "RimWorld installation not found. Expected: $assemblyPath. Set `$env:RIMWORLD_DIR to the RimWorld folder and run again."
}

$harmonyPath = $null
$harmonyCandidates = Get-ChildItem -Path (Join-Path $rimWorldDir 'Mods') -Recurse -Filter '0Harmony.dll' -File -ErrorAction SilentlyContinue
if ($harmonyCandidates.Count -gt 0) {
    $harmonyPath = $harmonyCandidates[0].FullName
}
if (-not $harmonyPath) {
    throw '0Harmony.dll not found under RimWorld\Mods. HSK requires Harmony, so make sure the HSK loadout is installed.'
}
$harmonyPath = $harmonyPath.TrimEnd('\', '/')

Write-Host "RimWorld found: $rimWorldDir"
Write-Host "Harmony found: $harmonyPath"
Write-Host "Building HSK Algae Cultivation..."

$project = Join-Path $sourceDir 'HSKAlgaeCultivation\HSKAlgaeCultivation.csproj'

dotnet build $project -c Release `
    -p:RimWorldDir="$rimWorldDir" `
    -p:HarmonyPath="$harmonyPath"

if ($LASTEXITCODE -ne 0) {
    throw 'BUILD FAILED.'
}

$dll = Join-Path $modDir '1.5\Assemblies\HSKAlgaeCultivation.dll'
Write-Host "BUILD SUCCESSFUL"
Write-Host "DLL: $dll"

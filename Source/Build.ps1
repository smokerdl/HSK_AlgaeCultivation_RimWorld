param(
    [string]$RimWorldVersion = $env:RIMWORLD_VERSION
)

$ErrorActionPreference = 'Stop'

if ([string]::IsNullOrWhiteSpace($RimWorldVersion)) {
    $RimWorldVersion = '1.5'
}

if ($RimWorldVersion -notin @('1.5', '1.6')) {
    throw "Unsupported RimWorld version '$RimWorldVersion'. Set RIMWORLD_VERSION to 1.5 or 1.6."
}

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
    throw "RimWorld installation not found. Expected: $assemblyPath. Set RIMWORLD_DIR to the RimWorld folder and run again."
}

$harmonyPath = $null
$preferredHarmony = Join-Path $rimWorldDir 'Mods\Core_SK\Assemblies\0Harmony.dll'
if (Test-Path $preferredHarmony) {
    $harmonyPath = $preferredHarmony
} else {
    $harmonyCandidates = Get-ChildItem -Path (Join-Path $rimWorldDir 'Mods') -Recurse -Filter '0Harmony.dll' -File -ErrorAction SilentlyContinue
    if ($harmonyCandidates.Count -gt 0) {
        $harmonyPath = $harmonyCandidates[0].FullName
    }
}

if (-not $harmonyPath) {
    throw '0Harmony.dll not found under RimWorld\Mods. Make sure the HSK loadout is installed.'
}

$harmonyPath = $harmonyPath.TrimEnd('\', '/')

Write-Host "RimWorld found: $rimWorldDir"
Write-Host "Target version folder: $RimWorldVersion"
Write-Host "Harmony found: $harmonyPath"
Write-Host "Building HSK Algae Cultivation..."

$project = Join-Path $sourceDir 'HSKAlgaeCultivation\HSKAlgaeCultivation.csproj'

dotnet build $project -c Release "-p:RimWorldDir=$rimWorldDir" "-p:RimWorldVersion=$RimWorldVersion" "-p:HarmonyPath=$harmonyPath"

if ($LASTEXITCODE -ne 0) {
    throw 'BUILD FAILED.'
}

$dll = Join-Path $modDir "$RimWorldVersion\Assemblies\HSKAlgaeCultivation.dll"
Write-Host "BUILD SUCCESSFUL"
Write-Host "DLL: $dll"

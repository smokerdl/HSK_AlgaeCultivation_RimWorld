param(
    [string]$RimWorldVersion = $env:RIMWORLD_VERSION
)

$ErrorActionPreference = 'Stop'

if ([string]::IsNullOrWhiteSpace($RimWorldVersion)) {
    $RimWorldVersion = '1.6'
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

$modsDir = Join-Path $rimWorldDir 'Mods'
$harmonyPath = $env:HARMONY_PATH

if ($harmonyPath) {
    if (-not (Test-Path $harmonyPath)) {
        throw "HARMONY_PATH was set but the file does not exist: $harmonyPath"
    }
} else {
    # Prefer known, deterministic HSK/Harmony locations.
    $preferredHarmonyPaths = @(
        (Join-Path $modsDir 'Core_SK\Assemblies\0Harmony.dll'),
        (Join-Path $modsDir 'Harmony\Current\Assemblies\0Harmony.dll'),
        (Join-Path $modsDir "Harmony\$RimWorldVersion\Assemblies\0Harmony.dll")
    )

    foreach ($candidate in $preferredHarmonyPaths) {
        if (Test-Path $candidate) {
            $harmonyPath = $candidate
            break
        }
    }

    if (-not $harmonyPath) {
        $allHarmonyCandidates = @(
            Get-ChildItem -Path $modsDir -Recurse -Filter '0Harmony.dll' -File -ErrorAction SilentlyContinue |
            Sort-Object FullName
        )

        # When Harmony is bundled in a version-specific mod folder, avoid
        # accidentally compiling against a DLL from the other game version.
        $versionPattern = "[\\/]" + [regex]::Escape($RimWorldVersion) + "[\\/]Assemblies[\\/]0Harmony\.dll$"
        $versionHarmonyCandidates = @(
            $allHarmonyCandidates | Where-Object { $_.FullName -match $versionPattern }
        )

        if ($versionHarmonyCandidates.Count -gt 0) {
            $harmonyPath = $versionHarmonyCandidates[0].FullName
            if ($versionHarmonyCandidates.Count -gt 1) {
                Write-Warning "Found $($versionHarmonyCandidates.Count) Harmony DLLs for RimWorld $RimWorldVersion; using the first sorted path. Set HARMONY_PATH to choose a specific DLL."
            }
        } else {
            $currentHarmonyCandidates = @(
                $allHarmonyCandidates | Where-Object { $_.FullName -match "[\\/]Current[\\/]Assemblies[\\/]0Harmony\.dll$" }
            )

            if ($currentHarmonyCandidates.Count -gt 0) {
                $harmonyPath = $currentHarmonyCandidates[0].FullName
            } elseif ($allHarmonyCandidates.Count -eq 1) {
                $harmonyPath = $allHarmonyCandidates[0].FullName
            } elseif ($allHarmonyCandidates.Count -gt 1) {
                $paths = ($allHarmonyCandidates | ForEach-Object { "  $($_.FullName)" }) -join [Environment]::NewLine
                throw ("Found multiple 0Harmony.dll files but none clearly matches RimWorld " + $RimWorldVersion + ". Set HARMONY_PATH to the correct file:" + [Environment]::NewLine + $paths)
            }
        }
    }
}

if (-not $harmonyPath) {
    throw '0Harmony.dll not found under RimWorld\Mods. Make sure the HSK loadout is installed or set HARMONY_PATH.'
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

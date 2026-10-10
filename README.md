# HSK Algae Cultivation

A small add-on for Hardcore SK (HSK) that adds a dedicated **Algae Cultivation** zone for HSK's existing `PlantAlgae` crop.

**Supported game versions:** RimWorld 1.5 and 1.6.

## Features

- Draw the zone on shallow water, moving shallow water, chest-deep moving water, and marsh.
- Automatic and forced sowing, plus harvesting.
- Optional SeedsPlease integration that adds algae seeds.
- Prebuilt game-version-specific assemblies are included; players do not need to compile the mod.

## Installation

1. Download the repository archive using **Code → Download ZIP**.
2. Extract the mod folder into `RimWorld/Mods/`.
3. Enable Hardcore SK and **HSK Algae Cultivation** in the mod list. Harmony must be available in the mod loadout.
4. Use **Zones → Algae Cultivation** in game.

Keep the `1.5` and `1.6` folders intact. RimWorld loads the files from the folder matching the installed game version.

SeedsPlease is optional. When package ID `notfood.seedsplease` is active, the mod defines algae seeds. Without SeedsPlease, the zone can still grow algae without seed mechanics.

## Build from source

Prebuilt DLLs are provided for normal play. To rebuild after changing C# code, run `Source/Build.bat` from the repository root on Windows. Set `RIMWORLD_DIR` to the correct game installation and `RIMWORLD_VERSION` to `1.5` or `1.6`. If needed, specify the exact Harmony assembly path with `HARMONY_PATH`.

For complete instructions and the manual in Russian, see [README_RU.md](README_RU.md).

## Validation

GitHub Actions validates XML syntax, required files for both game versions, matching version-specific patch/Def files, and localization keys. These static checks do not replace an in-game test after code or DLL changes.

# HSK Algae Cultivation

A small add-on for Hardcore SK (HSK). The active RimWorld 1.6 version provides a dedicated **Aquatic Crop Zone** for HSK algae and Odyssey reeds.

**Game versions:** the included RimWorld 1.5 build is a frozen legacy snapshot. New features and active testing target RimWorld 1.6.

## Features

- Draw the Aquatic Crop Zone on the mod's supported water and marsh terrain.
- Select between HSK's existing `PlantAlgae` and Odyssey's `Plant_Reeds` using the zone's crop selector.
- Automatic and forced sowing, plus harvesting for supported water crops.
- The active 1.6 version makes Odyssey reeds cultivable for hay: harvest begins at 33% growth and yields 6 Hay at full growth.
- The reeds patch is applied only when Odyssey is active, so algae cultivation remains usable without that DLC.
- Optional SeedsPlease integration, including seeds for algae and reeds.
- Prebuilt assemblies are included for normal play; players do not need to compile the mod.

## Installation

1. Download the repository archive using **Code → Download ZIP**.
2. Extract the mod folder into `RimWorld/Mods/`.
3. Enable Hardcore SK and **HSK Algae Cultivation** in the mod list. Harmony must be available in the mod loadout.
4. In RimWorld 1.6 with Odyssey enabled, use **Zones → Aquatic Crop Zone**.

Keep the `1.5` and `1.6` folders intact. The `1.5` content and assembly are retained as the previous build and will not receive new features.

SeedsPlease is optional. When package ID `notfood.seedsplease` is active, the mod defines seeds for supported water crops. Without SeedsPlease, the zone works without seed mechanics.

## Build from source

New development targets RimWorld 1.6. To rebuild after changing C# code, run `Source/Build.bat` from the repository root on Windows. Set `RIMWORLD_DIR` to the correct game installation and `RIMWORLD_VERSION=1.6`. The build script defaults to 1.6. If needed, specify the exact Harmony assembly path with `HARMONY_PATH`.

The prebuilt 1.5 DLL is retained as-is; do not rebuild the 1.5 target when you need to preserve that legacy snapshot.

For the manual in Russian, see [README_RU.md](README_RU.md).

## Validation

GitHub Actions validates XML syntax, required files for the active and legacy folders, and localization keys. Static checks do not replace an in-game test after code or DLL changes.

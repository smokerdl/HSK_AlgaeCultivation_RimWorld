# HSK Algae Cultivation

A focused add-on for Hardcore SK (HSK). The active RimWorld 1.6 version provides a dedicated **Aquatic Crop Zone** for cultivating HSK's existing algae (PlantAlgae).

**Game versions:** the included RimWorld 1.5 build is a frozen legacy snapshot. New features and active testing target RimWorld 1.6.

## Features

- Draw the Aquatic Crop Zone on the mod's supported water and marsh terrain.
- The crop selector explicitly offers HSK algae (PlantAlgae), which is the only supported crop for now.
- Automatic and forced sowing, harvesting, and saving/loading retain the selected algae crop.
- The zone uses existing HSK plant and harvest definitions; the add-on does not replace algae's graphics or harvest product.
- Optional SeedsPlease integration defines and supports algae seeds (Seed_Algae).

## Installation

1. Download the repository archive using **Code → Download ZIP**.
2. Extract the mod folder into RimWorld/Mods/.
3. Enable Hardcore SK and **HSK Algae Cultivation** in the mod list. Harmony must be available in the mod loadout.
4. In RimWorld 1.6, use **Zones → Aquatic Crop Zone** on supported water or marsh terrain.

Keep the 1.5 and 1.6 folders intact. The 1.5 content and assembly are retained as the previous build and will not receive new features.

SeedsPlease is optional. When package ID notfood.seedsplease is active, the mod defines seeds for algae. Without SeedsPlease, the zone still works without seed mechanics.

## Build from source

New development targets RimWorld 1.6. To rebuild after changing C# code, run Source/Build.bat from the repository root on Windows. Set RIMWORLD_DIR to the correct game installation and RIMWORLD_VERSION=1.6. The build script defaults to 1.6. If needed, specify the exact Harmony assembly path with HARMONY_PATH.

The prebuilt 1.5 DLL is retained as-is; do not rebuild the 1.5 target when you need to preserve that legacy snapshot.

For the manual in Russian, see [README_RU.md](README_RU.md).

## Validation

GitHub Actions validates XML syntax, required files for the active and legacy folders, and localization keys. Static checks do not replace an in-game test after code or DLL changes.

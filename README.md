# HSK Algae Cultivation

An add-on for Hardcore SK (HSK) that adds a dedicated aquatic crop zone for HSK's Algae and Water Lily in RimWorld 1.6.

**Version support:** active development targets RimWorld 1.6. The included RimWorld 1.5 folder and assembly remain a frozen legacy snapshot.

## Features

- A dedicated aquatic crop zone, labeled **Aquatic Crop Zone** in English and **Водорослевое хозяйство** in Russian, for supported water and marsh terrain.
- Two supported plants: HSK's `PlantAlgae` and the decorative `Plant_WaterLily`.
- RimWorld's standard crop selector. With SeedsPlease enabled, the relevant seed items must physically be present on the map for a crop to be offered by the selector.
- Water Lily has `Beauty=15` and `BeautyOutdoors=15`, uses original local textures, and is not harvested when mature.
- Optional SeedsPlease seed definitions for Algae and Water Lily.
- Algae sowing and harvesting with crop-aware cleanup; decorative Water Lily is excluded from harvest jobs.

## Cleanup and sowing behavior

- If a cell already contains the selected crop, the pawn leaves that plant in place. Naturally spawned Algae is preserved when Algae is selected.
- If a cell contains a different plant, the pawn first receives a `CutPlant` job. Sowing can occur on a subsequent work pass. Automatic cleanup requires both **Allow Sowing** and **Allow Cutting** to be enabled.
- If **Allow Cutting** is disabled, the pawn must not cut the mismatched plant or sow over it.
- Other planting blockers are handled separately: haulable items may be hauled aside, while non-movable blockers prevent sowing.

## Installation

1. Download the repository archive using **Code → Download ZIP**.
2. Extract the mod folder into `RimWorld/Mods/`.
3. Enable Hardcore SK, HSK Algae Cultivation, and Harmony. SeedsPlease is optional.
4. In RimWorld 1.6, choose **Zones → Aquatic Crop Zone** / **Зоны → Водорослевое хозяйство**.

Keep the `1.5` and `1.6` folders intact. The `1.5` files are kept for legacy purposes and are not updated.

## Build from source

New development targets RimWorld 1.6. To rebuild after changing C# code, run `Source/Build.bat` from the repository root on Windows. Set `RIMWORLD_DIR` to the game installation and `RIMWORLD_VERSION=1.6`. If needed, specify the Harmony assembly path with `HARMONY_PATH`.

The build and gameplay tests are important: the repository's GitHub Actions checks XML syntax, required files, and localization keys, but does not compile C# or launch RimWorld.

For the Russian guide, see [README_RU.md](README_RU.md).

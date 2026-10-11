# HSK Algae Cultivation

A small add-on for Hardcore SK (HSK). The active RimWorld 1.6 code provides an aquatic crop zone for HSK algae. The experimental `experiment/water-lily-standard-selector` branch also adds Water Lily to investigate why the HSK algae plant is missing from RimWorld's vanilla crop selector.

**Game versions:** the included RimWorld 1.5 build is a frozen legacy snapshot. New features and active testing target RimWorld 1.6.

## Features in the experiment branch

- Draw the Aquatic Crop Zone on the mod's supported water and marsh terrain.
- Register HSK's existing `PlantAlgae` and the test plant `Plant_WaterLily` as crops supported only by this custom zone.
- Use RimWorld's standard crop selector instead of a custom menu, so both plants can be compared through the same selection logic.
- Water Lily uses HSK's flower base and Beauty/BeautyOutdoors values of 15. Its texture currently reuses the HSK white-rose sprite as a placeholder.
- Optional SeedsPlease integration includes definitions for algae seeds and water lily seeds.
- Automatic/forced sowing and crop harvesting continue to use the mod's zone-specific workgiver patches.

## Installation and testing

1. Use the `experiment/water-lily-standard-selector` branch for this test, not `main`.
2. Install the mod in `RimWorld/Mods/` and enable Hardcore SK, HSK Algae Cultivation, Harmony, and optionally SeedsPlease.
3. In RimWorld 1.6, create an Aquatic Crop Zone on a supported water/marsh terrain.
4. Open the crop selector and check whether **Water Lily**, **Algae**, both, or neither are listed.
5. If the selector works, check sowing/growth, beauty stats, and SeedsPlease behavior separately. A visible selector entry does not by itself prove that the plant can grow on every allowed terrain.
6. Check the game log for XML errors and Harmony exceptions.

Keep the `1.5` and `1.6` folders intact. The `1.5` content and assembly remain a frozen legacy snapshot and will not receive new features.

SeedsPlease is optional. When package ID `notfood.seedsplease` is active, the mod loads its seed definitions. The flower texture is a temporary placeholder; replace it when custom Water Lily art is ready.

## Build from source

New development targets RimWorld 1.6. To rebuild after changing C# code, run `Source/Build.bat` from the repository root on Windows. Set `RIMWORLD_DIR` to the correct game installation and `RIMWORLD_VERSION=1.6`. The build script defaults to 1.6. If needed, specify the exact Harmony assembly path with `HARMONY_PATH`.

The prebuilt 1.5 DLL is retained as-is; do not rebuild the 1.5 target when you need to preserve that legacy snapshot.

For the manual in Russian, see [README_RU.md](README_RU.md).

## Validation

GitHub Actions validates XML syntax, required files for the active and legacy folders, and localization keys. Static checks do not replace a local C# build or an in-game test.

# HSK Algae Cultivation

A small add-on for Hardcore SK (HSK). The experimental RimWorld 1.6 branch adds Water Lily to test RimWorld's standard crop selector and the aquatic zone's cleanup behavior.

**Game versions:** the included RimWorld 1.5 build is a frozen legacy snapshot. New features and active testing target RimWorld 1.6.

## Features in the experiment branch

- An aquatic crop zone on the mod's supported water and marsh terrain.
- Two supported crops: HSK's `PlantAlgae` and the experimental `Plant_WaterLily`.
- RimWorld's standard crop selector, not a custom menu.
- Water Lily uses HSK's flower base with Beauty and BeautyOutdoors values of 15. Its texture temporarily reuses the HSK white-rose sprite.
- Optional SeedsPlease seed definitions for algae and water lily.
- Zone-specific automatic/forced sowing, crop cleanup, and harvesting logic.

## Expected cleanup behavior

- If a cell already contains the selected crop, leave that plant in place. For example, existing wild algae should be preserved when the selected crop is algae.
- If a cell contains a different plant, the pawn should first receive a CutPlant job; sowing may happen on a later work pass. Automatic cleanup requires both **Allow Sowing** and **Allow Cutting** to be enabled.
- When Allow Cutting is disabled, the pawn should neither cut the mismatched plant nor sow over it.
- Other planting blockers are handled separately. Haulable items may be hauled aside, while non-movable blockers prevent sowing.
- In the current SeedsPlease game test, the selected plant only appears in the selector when its seed items physically exist on the map. For selector testing, place algae seeds and/or water lily seeds on the map first.

This behavior still needs confirmation in-game with the updated build.

## Installation and testing

1. Use the `experiment/water-lily-standard-selector` branch for this test, not `main`.
2. Install the mod in `RimWorld/Mods/` and enable Hardcore SK, HSK Algae Cultivation, Harmony, and SeedsPlease for seed testing.
3. In RimWorld 1.6, create an Aquatic Crop Zone on supported terrain.
4. Check the selector with the relevant seed items present on the map.
5. Select algae and observe cells containing wild algae, Water Lily, Odyssey reeds (if Odyssey is enabled), another plant, and a haulable item.
6. Repeat with Allow Sowing and Allow Cutting enabled, then with Allow Cutting disabled.
7. Select algae in a cell that already contains wild algae and confirm the existing plant is preserved.
8. Check the game log for XML errors and Harmony exceptions.

Keep the `1.5` and `1.6` folders intact. The `1.5` content and assembly remain a frozen legacy snapshot and will not receive new features. The Water Lily texture is a placeholder pending custom art.

## Build from source

New development targets RimWorld 1.6. To rebuild after changing C# code, run `Source/Build.bat` from the repository root on Windows. Set `RIMWORLD_DIR` to the correct game installation and `RIMWORLD_VERSION=1.6`. If needed, specify the exact Harmony assembly path with `HARMONY_PATH`.

The prebuilt 1.5 DLL is retained as-is; do not rebuild the 1.5 target when you need to preserve that legacy snapshot.

For the manual in Russian, see [README_RU.md](README_RU.md).

## Validation

GitHub Actions validates XML syntax, required files for the active and legacy folders, and localization keys. Static checks do not replace a local C# build or an in-game test.

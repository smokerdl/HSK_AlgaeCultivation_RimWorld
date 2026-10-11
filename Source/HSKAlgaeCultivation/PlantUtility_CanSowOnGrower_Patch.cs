using HarmonyLib;
using RimWorld;
using Verse;

namespace HSKAlgaeCultivation
{
    [HarmonyPatch(typeof(PlantUtility), nameof(PlantUtility.CanSowOnGrower))]
    public static class PlantUtility_CanSowOnGrower_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(ThingDef plantDef, object obj, ref bool __result)
        {
            bool isAquaticCrop =
                plantDef == AlgaeDefOf.PlantAlgae ||
                plantDef == AlgaeDefOf.Plant_WaterLily;

            // Both test crops must be visible to the standard selector for
            // this zone, while remaining unavailable in ordinary growing zones.
            if (isAquaticCrop)
                __result = obj is Zone_GrowingAlgae;
            else if (obj is Zone_GrowingAlgae)
                __result = false;
        }
    }
}

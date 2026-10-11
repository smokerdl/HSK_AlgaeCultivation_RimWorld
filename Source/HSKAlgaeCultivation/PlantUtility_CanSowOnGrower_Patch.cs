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
            // Aquatic crops are allowed in the custom zone, and deliberately
            // rejected by ordinary ground zones/planters even though they carry
            // the Ground sow tag required by the vanilla picker and SeedsPlease.
            if (Zone_GrowingAlgae.IsSupportedCrop(plantDef))
                __result = obj is Zone_GrowingAlgae && Zone_GrowingAlgae.IsSupportedCrop(plantDef);
            else if (obj is Zone_GrowingAlgae)
                __result = false;
        }
    }
}

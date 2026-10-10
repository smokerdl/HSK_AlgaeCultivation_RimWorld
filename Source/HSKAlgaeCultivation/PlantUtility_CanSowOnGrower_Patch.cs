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
            if (obj is Zone_GrowingAlgae)
                __result = Zone_GrowingAlgae.IsSupportedCrop(plantDef);
        }
    }
}

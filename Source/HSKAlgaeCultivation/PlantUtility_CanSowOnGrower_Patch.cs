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
            // PlantAlgae needs Ground sowTags so the vanilla crop picker and
            // SeedsPlease can discover it, but it must only be cultivable in
            // this mod's aquatic zone.
            if (plantDef == AlgaeDefOf.PlantAlgae)
                __result = obj is Zone_GrowingAlgae;
            else if (obj is Zone_GrowingAlgae)
                __result = false;
        }
    }
}

using HarmonyLib;
using RimWorld;
using Verse;

namespace HSKAlgaeCultivation
{
    // The vanilla crop picker applies an additional research/availability filter
    // after ValidPlantTypesForGrowers. PlantAlgae should be selectable in this
    // custom zone even if its inherited sow research prerequisites are not met;
    // the zone-specific CanSowOnGrower patch still prevents it being offered by
    // ordinary ground growing zones.
    [HarmonyPatch(typeof(Command_SetPlantToGrow), nameof(Command_SetPlantToGrow.IsPlantAvailable))]
    public static class Command_SetPlantToGrow_IsPlantAvailable_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(ThingDef plantDef, Map map, ref bool __result)
        {
            if (__result || plantDef != AlgaeDefOf.PlantAlgae || map == null)
                return;

            foreach (Zone zone in map.zoneManager.AllZones)
            {
                if (zone is Zone_GrowingAlgae)
                {
                    __result = true;
                    return;
                }
            }
        }
    }
}

using HarmonyLib;
using RimWorld;
using Verse;

namespace HSKAlgaeCultivation
{
    // The vanilla crop selector filters candidates by sowing research and other
    // availability restrictions after ValidPlantTypesForGrowers. Preserve the
    // aquatic algae option whenever this zone is present on the map.
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

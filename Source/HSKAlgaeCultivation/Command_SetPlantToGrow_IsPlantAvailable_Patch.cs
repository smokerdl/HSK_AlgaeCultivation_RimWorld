using HarmonyLib;
using RimWorld;
using Verse;

namespace HSKAlgaeCultivation
{
    // The vanilla picker filters candidates by both CanSowOnGrower and
    // IsPlantAvailable. These aquatic crops may have inherited research gates
    // that should not hide them from the mod's zone selector.
    [HarmonyPatch(typeof(Command_SetPlantToGrow), nameof(Command_SetPlantToGrow.IsPlantAvailable))]
    public static class Command_SetPlantToGrow_IsPlantAvailable_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(ThingDef plantDef, Map map, ref bool __result)
        {
            if (__result || !Zone_GrowingAlgae.IsSupportedCrop(plantDef) || map == null)
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

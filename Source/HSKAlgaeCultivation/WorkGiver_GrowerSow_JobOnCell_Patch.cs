using HarmonyLib;
using RimWorld;
using Verse;

namespace HSKAlgaeCultivation
{
    [HarmonyPatch(typeof(WorkGiver_GrowerSow), nameof(WorkGiver_GrowerSow.JobOnCell))]
    public static class WorkGiver_GrowerSow_JobOnCell_Patch
    {
        [HarmonyPrefix]
        public static bool CheckAlgaeZone(IntVec3 c, Pawn pawn)
        {
            Zone_GrowingAlgae zone = c.GetZone(pawn.Map) as Zone_GrowingAlgae;
            return zone == null || zone.allowSow;
        }
    }
}

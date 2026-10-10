using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace HSKAlgaeCultivation
{
    [HarmonyPatch(typeof(WorkGiver_GrowerHarvest), nameof(WorkGiver_GrowerHarvest.HasJobOnCell))]
    public static class WorkGiver_GrowerHarvest_HasJobOnCell_Patch
    {
        [HarmonyPrefix]
        public static bool Prefix(Pawn pawn, IntVec3 c, bool forced, ref bool __result)
        {
            Zone_GrowingAlgae zone = c.GetZone(pawn.Map) as Zone_GrowingAlgae;
            if (zone == null)
                return true;

            Plant plant = c.GetPlant(pawn.Map);
            __result = CanHarvestWaterCrop(pawn, zone, forced, plant);
            return false;
        }

        internal static bool CanHarvestWaterCrop(
            Pawn pawn,
            Zone_GrowingAlgae zone,
            bool forced,
            Plant plant)
        {
            // The crop selector controls what gets sown, not whether existing
            // plants of another supported crop can be harvested after a switch.
            return zone.allowCut &&
                   plant != null &&
                   Zone_GrowingAlgae.IsSupportedCrop(plant.def) &&
                   !plant.IsForbidden(pawn) &&
                   plant.HarvestableNow &&
                   plant.LifeStage == PlantLifeStage.Mature &&
                   plant.CanYieldNow() &&
                   pawn.CanReserve(plant, 1, -1, null, forced);
        }
    }

    [HarmonyPatch(typeof(WorkGiver_GrowerHarvest), nameof(WorkGiver_GrowerHarvest.JobOnCell))]
    public static class WorkGiver_GrowerHarvest_JobOnCell_Patch
    {
        [HarmonyPrefix]
        public static bool Prefix(Pawn pawn, IntVec3 c, bool forced, ref Job __result)
        {
            Zone_GrowingAlgae zone = c.GetZone(pawn.Map) as Zone_GrowingAlgae;
            if (zone == null)
                return true;

            Plant plant = c.GetPlant(pawn.Map);
            if (WorkGiver_GrowerHarvest_HasJobOnCell_Patch.CanHarvestWaterCrop(pawn, zone, forced, plant))
                __result = JobMaker.MakeJob(JobDefOf.Harvest, plant);
            else
                __result = null;

            return false;
        }
    }
}
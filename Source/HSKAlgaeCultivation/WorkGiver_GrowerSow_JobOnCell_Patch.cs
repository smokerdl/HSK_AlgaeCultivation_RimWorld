using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace HSKAlgaeCultivation
{
    [HarmonyPatch(typeof(WorkGiver_GrowerSow), nameof(WorkGiver_GrowerSow.JobOnCell))]
    public static class WorkGiver_GrowerSow_JobOnCell_Patch
    {
        [HarmonyPrefix]
        public static bool Prefix(Pawn pawn, IntVec3 c, bool forced, ref Job __result)
        {
            Zone_GrowingAlgae zone = c.GetZone(pawn.Map) as Zone_GrowingAlgae;
            if (zone == null)
                return true;

            __result = TryMakeWaterCropSowJob(pawn, c, zone, forced);
            return false;
        }

        private static Job TryMakeWaterCropSowJob(Pawn pawn, IntVec3 c, Zone_GrowingAlgae zone, bool forced)
        {
            if (!zone.allowSow || c.IsForbidden(pawn))
                return null;

            ThingDef plantDef = zone.GetPlantDefToGrow();
            if (!Zone_GrowingAlgae.IsSupportedCrop(plantDef))
                return null;

#if RIMWORLD_1_6
            if (!PlantUtility.GrowthSeasonNow(c, pawn.Map, plantDef))
                return null;
#else
            if (!PlantUtility.GrowthSeasonNow(c, pawn.Map, true))
                return null;
#endif

            // Match vanilla grow-zone behavior: do not disturb a plant that
            // already matches the selected crop. This includes naturally
            // spawned algae when algae is the selected crop.
            Plant existingPlant = c.GetPlant(pawn.Map);
            if (existingPlant != null)
            {
                if (existingPlant.def == plantDef)
                    return null;

                // Plants may not report BlockPlanting in the same way as other
                // blockers. Handle mismatched plants explicitly, and respect
                // the zone's Allow Cutting toggle before scheduling removal.
                if (!zone.allowCut ||
                    existingPlant.IsForbidden(pawn) ||
                    !pawn.CanReserve(existingPlant, 1, -1, null, forced))
                {
                    return null;
                }

                return JobMaker.MakeJob(JobDefOf.CutPlant, existingPlant);
            }

            List<Thing> thingList = c.GetThingList(pawn.Map);

            for (int i = 0; i < thingList.Count; i++)
            {
                Thing thing = thingList[i];

                // A plant at this cell was handled above, so only deal with
                // buildings, items, blueprints, and other planting blockers here.
                if (!thing.def.BlocksPlanting(true))
                    continue;

                if (!pawn.CanReserve(thing, 1, -1, null, forced))
                    return null;

                if (thing.def.category == ThingCategory.Plant)
                {
                    // Defensive fallback in case a custom ThingDef is not
                    // returned by c.GetPlant for some reason.
                    if (thing is Plant plant &&
                        zone.allowCut &&
                        !thing.IsForbidden(pawn))
                    {
                        return JobMaker.MakeJob(JobDefOf.CutPlant, plant);
                    }

                    return null;
                }

                if (thing.def.EverHaulable)
                    return HaulAIUtility.HaulAsideJobFor(pawn, thing);

                return null;
            }

            if (plantDef.plant.sowMinSkill > 0 &&
                pawn.skills != null &&
                pawn.skills.GetSkill(SkillDefOf.Plants).Level < plantDef.plant.sowMinSkill)
            {
                return null;
            }

            if (!pawn.CanReserve(c, 1, -1, null, forced))
                return null;

            Job job = JobMaker.MakeJob(JobDefOf.Sow, c);
            job.plantDefToSow = plantDef;
            return job;
        }
    }
}

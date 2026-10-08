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

            __result = TryMakeAlgaeSowJob(pawn, c, zone, forced);
            return false;
        }

        private static Job TryMakeAlgaeSowJob(Pawn pawn, IntVec3 c, Zone_GrowingAlgae zone, bool forced)
        {
            if (!zone.allowSow || c.IsForbidden(pawn))
                return null;

            if (!PlantUtility.GrowthSeasonNow(c, pawn.Map, true))
                return null;

            ThingDef plantDef = AlgaeDefOf.PlantAlgae;
            List<Thing> thingList = c.GetThingList(pawn.Map);

            for (int i = 0; i < thingList.Count; i++)
            {
                Thing thing = thingList[i];

                if (thing.def == plantDef)
                    return null;

                if (!thing.def.BlocksPlanting(true))
                    continue;

                if (!pawn.CanReserve(thing, 1, -1, null, forced))
                    return null;

                if (thing.def.category == ThingCategory.Plant)
                {
                    Plant plant = thing as Plant;
                    if (!thing.IsForbidden(pawn) && PlantUtility.PawnWillingToCutPlant_Job(plant, pawn))
                        return JobMaker.MakeJob(JobDefOf.CutPlant, thing);

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

using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace HSKAlgaeCultivation
{
    public class WorkGiver_GrowerSowAlgae : WorkGiver_GrowerAlgae
    {
        public override PathEndMode PathEndMode => PathEndMode.ClosestTouch;

        protected override bool ExtraRequirements(IPlantToGrowSettable settable, Pawn pawn)
        {
            if (!settable.CanAcceptSowNow())
                return false;

            var zone = settable as Zone_GrowingAlgae;
            if (zone == null || !zone.allowSow || zone.Cells.Count == 0)
                return false;

            wantedPlantDef = CalculateWantedPlantDef(zone.Cells[0], pawn.Map);
            return wantedPlantDef == AlgaeDefOf.PlantAlgae;
        }

        public override Job JobOnCell(Pawn pawn, IntVec3 c, bool forced = false)
        {
            Map map = pawn.Map;

            if (c.IsForbidden(pawn))
                return null;

            if (!PlantUtility.GrowthSeasonNow(c, map, true))
                return null;

            var zone = c.GetZone(map) as Zone_GrowingAlgae;
            if (zone == null || !zone.allowSow)
                return null;

            wantedPlantDef = CalculateWantedPlantDef(c, map);
            if (wantedPlantDef != AlgaeDefOf.PlantAlgae)
                return null;

            List<Thing> thingList = c.GetThingList(map);

            for (int i = 0; i < thingList.Count; i++)
            {
                Thing thing = thingList[i];

                if (thing.def == wantedPlantDef)
                    return null;

                if (thing.def.BlocksPlanting(true))
                {
                    if (!pawn.CanReserve(thing, 1, -1, null, forced))
                        return null;

                    if (thing.def.category == ThingCategory.Plant)
                    {
                        if (!thing.IsForbidden(pawn) && PlantUtility.PawnWillingToCutPlant_Job(thing as Plant, pawn))
                            return JobMaker.MakeJob(JobDefOf.CutPlant, thing);

                        return null;
                    }

                    if (thing.def.EverHaulable)
                        return HaulAIUtility.HaulAsideJobFor(pawn, thing);

                    return null;
                }
            }

            if (wantedPlantDef.plant.sowMinSkill > 0 &&
                pawn.skills != null &&
                pawn.skills.GetSkill(SkillDefOf.Plants).Level < wantedPlantDef.plant.sowMinSkill)
            {
                return null;
            }

            if (!wantedPlantDef.CanNowPlantAt(c, map) ||
                !PlantUtility.GrowthSeasonNow(c, map, true) ||
                !pawn.CanReserve(c, 1, -1, null, forced))
            {
                return null;
            }

            ThingDef seedDef = wantedPlantDef.blueprintDef;
            JobDef sowWithSeeds = DefDatabase<JobDef>.GetNamedSilentFail("SowWithSeeds");

            if (seedDef != null && sowWithSeeds != null)
            {
                Predicate<Thing> validator = thing =>
                    !thing.IsForbidden(pawn) &&
                    pawn.AllowedArea != null ? pawn.AllowedArea[thing.Position] : true;

                Thing seed = null;
                var seedThings = map.listerThings.ThingsOfDef(seedDef);
                for (int i = 0; i < seedThings.Count; i++)
                {
                    Thing candidate = seedThings[i];
                    if (candidate.IsForbidden(pawn))
                        continue;
                    if (pawn.AllowedArea != null && !pawn.AllowedArea[candidate.Position])
                        continue;
                    if (!pawn.CanReach(candidate, PathEndMode.ClosestTouch, Danger.Deadly))
                        continue;
                    if (!ReservationUtility.CanReserve(pawn, candidate, 1))
                        continue;

                    seed = candidate;
                    break;
                }

                if (seed == null)
                    return null;

                return new Job(sowWithSeeds, c, seed)
                {
                    plantDefToSow = wantedPlantDef,
                    count = 25
                };
            }

            Job job = JobMaker.MakeJob(JobDefOf.Sow, c);
            job.plantDefToSow = wantedPlantDef;
            return job;
        }
    }
}

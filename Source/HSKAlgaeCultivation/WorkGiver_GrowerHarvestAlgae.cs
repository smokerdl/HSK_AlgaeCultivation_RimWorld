using RimWorld;
using Verse;
using Verse.AI;

namespace HSKAlgaeCultivation
{
    public class WorkGiver_GrowerHarvestAlgae : WorkGiver_GrowerAlgae
    {
        public override PathEndMode PathEndMode => PathEndMode.Touch;

        public override bool HasJobOnCell(Pawn pawn, IntVec3 c, bool forced = false)
        {
            var zone = c.GetZone(pawn.Map) as Zone_GrowingAlgae;
            if (zone == null || !zone.allowCut)
                return false;

            Plant plant = c.GetPlant(pawn.Map);
            if (plant == null ||
                plant.def != zone.GetPlantDefToGrow() ||
                plant.IsForbidden(pawn) ||
                !plant.HarvestableNow ||
                plant.LifeStage != PlantLifeStage.Mature ||
                !plant.CanYieldNow() ||
                (!plant.def.plant.autoHarvestable && !forced))
            {
                return false;
            }

            return PlantUtility.PawnWillingToCutPlant_Job(plant, pawn) &&
                   pawn.CanReserve(plant, 1, -1, null, forced);
        }

        public override Job JobOnCell(Pawn pawn, IntVec3 c, bool forced = false)
        {
            if (!HasJobOnCell(pawn, c, forced))
                return null;

            return JobMaker.MakeJob(JobDefOf.Harvest, c.GetPlant(pawn.Map));
        }
    }
}

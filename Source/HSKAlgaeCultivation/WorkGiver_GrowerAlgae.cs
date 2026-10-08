using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace HSKAlgaeCultivation
{
    public abstract class WorkGiver_GrowerAlgae : WorkGiver_Scanner
    {
        protected static ThingDef wantedPlantDef;

        public override bool AllowUnreachable => true;

        protected virtual bool ExtraRequirements(IPlantToGrowSettable settable, Pawn pawn)
        {
            return true;
        }

        public override IEnumerable<IntVec3> PotentialWorkCellsGlobal(Pawn pawn)
        {
            if (pawn?.Map == null)
                yield break;

            Danger maxDanger = pawn.NormalMaxDanger();
            var zones = pawn.Map.zoneManager.AllZones;

            for (int i = 0; i < zones.Count; i++)
            {
                var zone = zones[i] as Zone_GrowingAlgae;
                if (zone == null || zone.Cells.Count == 0 || zone.ContainsStaticFire)
                    continue;

                if (!ExtraRequirements(zone, pawn))
                    continue;

                if (!pawn.CanReach(zone.Cells[0], PathEndMode.OnCell, maxDanger))
                    continue;

                for (int j = 0; j < zone.Cells.Count; j++)
                    yield return zone.Cells[j];

                wantedPlantDef = null;
            }

            wantedPlantDef = null;
        }

        public static ThingDef CalculateWantedPlantDef(IntVec3 c, Map map)
        {
            return c.GetPlantToGrowSettable(map)?.GetPlantDefToGrow();
        }
    }
}

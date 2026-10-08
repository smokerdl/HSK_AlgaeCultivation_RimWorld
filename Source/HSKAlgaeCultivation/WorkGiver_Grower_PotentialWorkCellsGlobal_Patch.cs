using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse;

namespace HSKAlgaeCultivation
{
    [HarmonyPatch(typeof(WorkGiver_Grower), nameof(WorkGiver_Grower.PotentialWorkCellsGlobal))]
    public static class WorkGiver_Grower_PotentialWorkCellsGlobal_Patch
    {
        [HarmonyPostfix]
        public static void AddAlgaeZones(Pawn pawn, ref IEnumerable<IntVec3> __result)
        {
            IEnumerable<IntVec3> original = __result;
            __result = AddAlgaeZoneCells(original, pawn);
        }

        private static IEnumerable<IntVec3> AddAlgaeZoneCells(IEnumerable<IntVec3> original, Pawn pawn)
        {
            if (original != null)
            {
                foreach (IntVec3 cell in original)
                    yield return cell;
            }

            if (pawn?.Map == null)
                yield break;

            Danger maxDanger = pawn.NormalMaxDanger();
            List<Zone> zones = pawn.Map.zoneManager.AllZones;

            for (int i = 0; i < zones.Count; i++)
            {
                Zone_GrowingAlgae zone = zones[i] as Zone_GrowingAlgae;
                if (zone == null || zone.Cells.Count == 0 || zone.ContainsStaticFire)
                    continue;

                for (int j = 0; j < zone.Cells.Count; j++)
                    yield return zone.Cells[j];
            }
        }
    }
}

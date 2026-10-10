using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace HSKAlgaeCultivation
{
    [HarmonyPatch(typeof(WorkGiver_Grower), nameof(WorkGiver_Grower.PotentialWorkCellsGlobal))]
    public static class WorkGiver_Grower_PotentialWorkCellsGlobal_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(
            WorkGiver_Grower __instance,
            Pawn pawn,
            ref IEnumerable<IntVec3> __result)
        {
            if (pawn?.Map == null)
                return;

            // Keep the vanilla growing workgivers, but guarantee algae-zone cells
            // are available to the same-cell manual-order menu as well.
            if (!(__instance is WorkGiver_GrowerSow) &&
                !(__instance is WorkGiver_GrowerHarvest))
            {
                return;
            }

            __result = AppendAlgaeZoneCells(__result, pawn);
        }

        private static IEnumerable<IntVec3> AppendAlgaeZoneCells(
            IEnumerable<IntVec3> original,
            Pawn pawn)
        {
            var yielded = new HashSet<IntVec3>();

            if (original != null)
            {
                foreach (IntVec3 cell in original)
                {
                    if (yielded.Add(cell))
                        yield return cell;
                }
            }

            List<Zone> zones = pawn.Map.zoneManager.AllZones;
            for (int i = 0; i < zones.Count; i++)
            {
                Zone_GrowingAlgae zone = zones[i] as Zone_GrowingAlgae;
                if (zone == null || zone.Cells.Count == 0 || zone.ContainsStaticFire)
                    continue;

                for (int j = 0; j < zone.Cells.Count; j++)
                {
                    IntVec3 cell = zone.Cells[j];
                    if (yielded.Add(cell))
                        yield return cell;
                }
            }
        }
    }
}
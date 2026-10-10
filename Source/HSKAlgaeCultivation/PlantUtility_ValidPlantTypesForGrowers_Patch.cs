using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse;

namespace HSKAlgaeCultivation
{
    // PlantUtility.CanSowOnGrower alone is not sufficient to make a plant
    // visible in every crop selector scenario. Explicitly include the crops
    // supported by this zone in the selector's candidate list.
    [HarmonyPatch(typeof(PlantUtility), nameof(PlantUtility.ValidPlantTypesForGrowers))]
    public static class PlantUtility_ValidPlantTypesForGrowers_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(
            List<IPlantToGrowSettable> sel,
            ref IEnumerable<ThingDef> __result)
        {
            if (sel == null || sel.Count == 0)
                return;

            // Do not expose aquatic crops for ordinary soil zones, planters,
            // or mixed selections involving other grower types.
            for (int i = 0; i < sel.Count; i++)
            {
                if (!(sel[i] is Zone_GrowingAlgae))
                    return;
            }

            var available = new List<ThingDef>();
            if (__result != null)
            {
                foreach (ThingDef plantDef in __result)
                {
                    if (plantDef != null && !available.Contains(plantDef))
                        available.Add(plantDef);
                }
            }

            AddSupportedCrop(available, AlgaeDefOf.PlantAlgae);
#if RIMWORLD_1_6
            AddSupportedCrop(available, AlgaeDefOf.Plant_Reeds);
#endif
            __result = available;
        }

        private static void AddSupportedCrop(List<ThingDef> available, ThingDef plantDef)
        {
            if (Zone_GrowingAlgae.IsSupportedCrop(plantDef) && !available.Contains(plantDef))
                available.Add(plantDef);
        }
    }
}

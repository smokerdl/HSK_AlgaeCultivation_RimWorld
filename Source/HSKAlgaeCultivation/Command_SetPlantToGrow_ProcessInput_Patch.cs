using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse;

namespace HSKAlgaeCultivation
{
    // Core_SK defines PlantAlgae as a general plant instead of a CropPlant and
    // does not give it the sow tags used by normal cultivated plants. The
    // vanilla crop picker therefore filters it out inconsistently. This
    // explicit picker is limited to our aquatic zone and currently offers
    // the only supported crop: HSK algae.
    [HarmonyPatch(typeof(Command_SetPlantToGrow), nameof(Command_SetPlantToGrow.ProcessInput))]
    public static class Command_SetPlantToGrow_ProcessInput_Patch
    {
        [HarmonyPrefix]
        public static bool Prefix(Command_SetPlantToGrow __instance)
        {
            Zone_GrowingAlgae zone = __instance.settable as Zone_GrowingAlgae;
            if (zone == null)
                return true;

            ThingDef algae = AlgaeDefOf.PlantAlgae;
            List<FloatMenuOption> options = new List<FloatMenuOption>
            {
                new FloatMenuOption(algae.LabelCap, delegate
                {
                    zone.SetPlantDefToGrow(algae);
                }, algae)
            };

            Find.WindowStack.Add(new FloatMenu(options));
            return false;
        }
    }
}

using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace HSKAlgaeCultivation
{
    // Core_SK defines PlantAlgae as a general plant instead of a CropPlant and
    // does not give it the sow tags used by normal cultivated plants. Rather
    // than relying on the vanilla filters to populate this zone's list, show
    // the one supported crop explicitly. This patch only intercepts our zone.
    [HarmonyPatch(typeof(Command_SetPlantToGrow), nameof(Command_SetPlantToGrow.ProcessInput))]
    public static class Command_SetPlantToGrow_ProcessInput_Patch
    {
        [HarmonyPrefix]
        public static bool Prefix(Command_SetPlantToGrow __instance, Event ev)
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

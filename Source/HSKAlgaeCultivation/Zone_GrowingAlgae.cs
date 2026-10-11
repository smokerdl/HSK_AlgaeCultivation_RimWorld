using System.Collections.Generic;
using UnityEngine;
using RimWorld;
using Verse;

namespace HSKAlgaeCultivation
{
    // Keep this class name so existing saves can still load their algae zones.
    public class Zone_GrowingAlgae : Zone_Growing, IPlantToGrowSettable
    {
        private static readonly List<Color> ZoneColors = new List<Color>();
        private static int nextColorIndex;
        private ThingDef cropToGrow;

        IEnumerable<IntVec3> IPlantToGrowSettable.Cells => Cells;

        protected override Color NextZoneColor => NextAlgaeZoneColor();

        public Zone_GrowingAlgae()
        {
        }

        public Zone_GrowingAlgae(ZoneManager zoneManager)
            : base(zoneManager)
        {
            label = "HSKAlgaeCultivation_ZoneLabel".Translate();
            cropToGrow = AlgaeDefOf.PlantAlgae;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Defs.Look(ref cropToGrow, "hskAlgaeCultivationCropToGrow");

            // Preserve supported crop selections in new saves. Old or unknown
            // saved values fall back to algae to maintain compatibility.
            if (Scribe.mode == LoadSaveMode.PostLoadInit && !IsSupportedCrop(cropToGrow))
                cropToGrow = AlgaeDefOf.PlantAlgae;
        }

        private static Color NextAlgaeZoneColor()
        {
            ZoneColors.Clear();
            ZoneColors.Add(new Color(0.05f, 0.55f, 0.75f, 0.09f));
            ZoneColors.Add(new Color(0.10f, 0.70f, 0.55f, 0.09f));
            ZoneColors.Add(new Color(0.05f, 0.40f, 0.90f, 0.09f));

            Color result = ZoneColors[nextColorIndex];
            nextColorIndex = (nextColorIndex + 1) % ZoneColors.Count;
            return result;
        }

        public static bool IsSupportedCrop(ThingDef plantDef)
        {
            return plantDef != null &&
                   (plantDef == AlgaeDefOf.PlantAlgae ||
                    plantDef == AlgaeDefOf.Plant_WaterLily);
        }

        public override string GetInspectString()
        {
            ThingDef selectedCrop = GetPlantDefToGrow();
            return "HSKAlgaeCultivation_ZoneInspect".Translate(selectedCrop.LabelCap);
        }

        public override void AddCell(IntVec3 c)
        {
            base.AddCell(c);
            foreach (Thing thing in Map.thingGrid.ThingsListAt(c))
            {
                Designator_PlantsHarvestWood.PossiblyWarnPlayerImportantPlantDesignateCut(thing);
            }
        }

        public override IEnumerable<Gizmo> GetZoneAddGizmos()
        {
            yield return DesignatorUtility.FindAllowedDesignator<Designator_AlgaeGrowingZone_Expand>();
        }

        // Reimplement IPlantToGrowSettable while leaving Zone_Growing's private
        // plant field alone. The vanilla crop picker calls this implementation.
        public new ThingDef GetPlantDefToGrow()
        {
            return IsSupportedCrop(cropToGrow) ? cropToGrow : AlgaeDefOf.PlantAlgae;
        }

        public new void SetPlantDefToGrow(ThingDef plantDef)
        {
            if (IsSupportedCrop(plantDef))
                cropToGrow = plantDef;
        }

        public new bool CanAcceptSowNow()
        {
            return allowSow;
        }
    }
}

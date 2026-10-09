using System.Collections.Generic;
using UnityEngine;
using RimWorld;
using Verse;

namespace HSKAlgaeCultivation
{
    public class Zone_GrowingAlgae : Zone_Growing, IPlantToGrowSettable
    {
        private static readonly List<Color> ZoneColors = new List<Color>();
        private static int nextColorIndex;

        IEnumerable<IntVec3> IPlantToGrowSettable.Cells => Cells;

        protected override Color NextZoneColor => NextAlgaeZoneColor();

        public Zone_GrowingAlgae()
        {
        }

        public Zone_GrowingAlgae(ZoneManager zoneManager)
            : base(zoneManager)
        {
            label = "HSKAlgaeCultivation_ZoneLabel".Translate();
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

        public override void ExposeData()
        {
            base.ExposeData();
        }

        public override string GetInspectString()
        {
            return "HSKAlgaeCultivation_ZoneInspect".Translate(AlgaeDefOf.PlantAlgae.LabelCap);
        }

        public override void AddCell(IntVec3 c)
        {
            base.AddCell(c);
            foreach (Thing thing in Map.thingGrid.ThingsListAt(c))
            {
                Designator_PlantsHarvestWood.PossiblyWarnPlayerImportantPlantDesignateCut(thing);
            }
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            foreach (Gizmo gizmo in base.GetGizmos())
                yield return gizmo;
        }

        public override IEnumerable<Gizmo> GetZoneAddGizmos()
        {
            yield return DesignatorUtility.FindAllowedDesignator<Designator_AlgaeGrowingZone_Expand>();
        }

        public new ThingDef GetPlantDefToGrow()
        {
            return AlgaeDefOf.PlantAlgae;
        }

        public new void SetPlantDefToGrow(ThingDef plantDef)
        {
            // The zone is intentionally restricted to HSK's algae crop.
        }

        public new bool CanAcceptSowNow()
        {
            return allowSow;
        }
    }
}
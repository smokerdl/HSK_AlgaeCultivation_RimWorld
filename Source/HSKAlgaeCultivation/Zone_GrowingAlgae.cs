using System.Collections.Generic;
using UnityEngine;
using RimWorld;
using Verse;

namespace HSKAlgaeCultivation
{
    public class Zone_GrowingAlgae : Zone_Growing, IPlantToGrowSettable
    {
        public bool allowCut = true;

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
            label = zoneManager.NewZoneName("HSKAlgaeCultivation_ZoneLabel".Translate());
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
            Scribe_Values.Look(ref allowCut, "allowCut", true);
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

            yield return new Command_Toggle
            {
                defaultLabel = "CommandAllowCut".Translate(),
                defaultDesc = "CommandAllowCutDesc".Translate(),
                icon = Designator_PlantsCut.IconTex,
                isActive = () => allowCut,
                toggleAction = () => allowCut = !allowCut
            };
        }

        public override IEnumerable<Gizmo> GetZoneAddGizmos()
        {
            yield return DesignatorUtility.FindAllowedDesignator<Designator_AlgaeGrowingZone_Expand>();
        }

        public ThingDef GetPlantDefToGrow()
        {
            return AlgaeDefOf.PlantAlgae;
        }

        public void SetPlantDefToGrow(ThingDef plantDef)
        {
            // The zone is intentionally restricted to HSK's algae crop.
        }

        public bool CanAcceptSowNow()
        {
            return allowSow;
        }
    }
}
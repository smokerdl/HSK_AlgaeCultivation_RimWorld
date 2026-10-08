using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using RimWorld;
using Verse;

namespace HSKAlgaeCultivation
{
    public class Zone_GrowingAlgae : Zone, IPlantToGrowSettable
    {
        private ThingDef plantDefToGrow = AlgaeDefOf.PlantAlgae;
        public bool allowSow = true;
        public bool allowCut = true;

        private static readonly List<Color> ZoneColors = new List<Color>();
        private static int nextColorIndex;

        public override bool IsMultiselectable => true;

        IEnumerable<IntVec3> IPlantToGrowSettable.Cells => Cells;

        protected override Color NextZoneColor => NextAlgaeZoneColor();

        public Zone_GrowingAlgae() { }

        public Zone_GrowingAlgae(ZoneManager zoneManager)
            : base("HSKAlgaeCultivation_ZoneLabel".Translate(), zoneManager)
        {
        }

        private static Color NextAlgaeZoneColor()
        {
            ZoneColors.Clear();
            ZoneColors.Add(new Color(0.05f, 0.55f, 0.75f, 0.09f));
            ZoneColors.Add(new Color(0.10f, 0.70f, 0.55f, 0.09f));
            ZoneColors.Add(new Color(0.05f, 0.40f, 0.90f, 0.09f));

            var result = ZoneColors[nextColorIndex];
            nextColorIndex = (nextColorIndex + 1) % ZoneColors.Count;
            return result;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Defs.Look(ref plantDefToGrow, "plantDefToGrow");
            Scribe_Values.Look(ref allowSow, "allowSow", true);
            Scribe_Values.Look(ref allowCut, "allowCut", true);
        }

        public override string GetInspectString()
        {
            var label = plantDefToGrow?.LabelCap ?? AlgaeDefOf.PlantAlgae.LabelCap;
            return "HSKAlgaeCultivation_ZoneInspect".Translate(label);
        }

        public override void AddCell(IntVec3 c)
        {
            base.AddCell(c);
            foreach (var thing in Map.thingGrid.ThingsListAt(c))
            {
                Designator_PlantsHarvestWood.PossiblyWarnPlayerImportantPlantDesignateCut(thing);
            }
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            foreach (var gizmo in base.GetGizmos())
                yield return gizmo;

            yield return PlantToGrowSettableUtility.SetPlantToGrowCommand(this);

            yield return new Command_Toggle
            {
                defaultLabel = "CommandAllowSow".Translate(),
                defaultDesc = "CommandAllowSowDesc".Translate(),
                hotKey = KeyBindingDefOf.Command_ItemForbid,
                icon = TexCommand.ForbidOff,
                isActive = () => allowSow,
                toggleAction = () => allowSow = !allowSow
            };

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

        public ThingDef GetPlantDefToGrow() => plantDefToGrow ?? AlgaeDefOf.PlantAlgae;

        public void SetPlantDefToGrow(ThingDef plantDef)
        {
            // The zone is intentionally restricted to HSK's algae crop.
            plantDefToGrow = AlgaeDefOf.PlantAlgae;
        }

        public bool CanAcceptSowNow() => allowSow;
    }
}

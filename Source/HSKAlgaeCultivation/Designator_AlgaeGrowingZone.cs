using UnityEngine;
using RimWorld;
using Verse;

namespace HSKAlgaeCultivation
{
    public class Designator_AlgaeGrowingZone : Designator_ZoneAdd
    {
        private static readonly string[] AllowedTerrains =
        {
            "WaterMovingChestDeep",
            "WaterShallow",
            "WaterMovingShallow",
            "Marsh"
        };

        protected override string NewZoneLabel => defaultLabel;

        public Designator_AlgaeGrowingZone()
        {
            zoneTypeToPlace = typeof(Zone_GrowingAlgae);
            defaultLabel = "HSKAlgaeCultivation_ZoneLabel".Translate();
            defaultDesc = "HSKAlgaeCultivation_ZoneDesc".Translate();
            icon = ContentFinder<Texture2D>.Get("UI/Designators/ZoneCreate_Growing", true);
            hotKey = KeyBindingDefOf.Misc2;
            tutorTag = "ZoneAdd_Growing";
        }

        public override AcceptanceReport CanDesignateCell(IntVec3 c)
        {
            if (!base.CanDesignateCell(c).Accepted)
                return false;

            if (!c.Walkable(Map))
                return false;

            var terrain = Map.terrainGrid.TerrainAt(c);
            for (int i = 0; i < AllowedTerrains.Length; i++)
            {
                if (terrain.defName == AllowedTerrains[i])
                    return true;
            }

            return false;
        }

        protected override Zone MakeNewZone()
        {
            PlayerKnowledgeDatabase.KnowledgeDemonstrated(ConceptDefOf.GrowingFood, KnowledgeAmount.Total);
            return new Zone_GrowingAlgae(Find.CurrentMap.zoneManager);
        }
    }
}

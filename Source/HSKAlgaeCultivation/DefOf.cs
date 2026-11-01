using RimWorld;
using Verse;

namespace HSKAlgaeCultivation
{
    [DefOf]
    public static class AlgaeDefOf
    {
        public static ThingDef PlantAlgae;
        public static ThingDef Plant_WaterLily;
        public static ThingDef Plant_Glowreed;

        static AlgaeDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(AlgaeDefOf));
        }
    }
}

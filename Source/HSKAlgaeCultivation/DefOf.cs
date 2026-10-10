using RimWorld;
using Verse;

namespace HSKAlgaeCultivation
{
    [DefOf]
    public static class AlgaeDefOf
    {
        public static ThingDef PlantAlgae;

#if RIMWORLD_1_6
        public static ThingDef Plant_Reeds;
#endif

        static AlgaeDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(AlgaeDefOf));
        }
    }
}

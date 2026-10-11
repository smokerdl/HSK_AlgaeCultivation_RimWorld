using RimWorld;
using Verse;

namespace HSKAlgaeCultivation
{
    [DefOf]
    public static class AlgaeDefOf
    {
        public static ThingDef PlantAlgae;

        static AlgaeDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(AlgaeDefOf));
        }
    }
}

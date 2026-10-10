using RimWorld;
using Verse;

namespace HSKAlgaeCultivation
{
    [DefOf]
    public static class AlgaeDefOf
    {
        public static ThingDef PlantAlgae;

#if RIMWORLD_1_6
        // Odyssey is optional for the algae zone. Resolve its plant without
        // registering an optional DLC DefOf field that would be missing when
        // Odyssey is not active.
        public static ThingDef Plant_Reeds => DefDatabase<ThingDef>.GetNamedSilentFail("Plant_Reeds");
#endif

        static AlgaeDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(AlgaeDefOf));
        }
    }
}

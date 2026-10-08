using System.Reflection;
using HarmonyLib;
using Verse;

namespace HSKAlgaeCultivation
{
    [StaticConstructorOnStartup]
    public static class Main
    {
        static Main()
        {
            var harmony = new Harmony("smokerdl.hsk.algaecultivation");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            Log.Message("[HSK Algae Cultivation] Loaded.");
        }
    }
}

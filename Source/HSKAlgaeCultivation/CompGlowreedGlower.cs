using HarmonyLib;
using UnityEngine;
using Verse;

namespace HSKAlgaeCultivation
{
    /// <summary>
    /// Stores sunlight during the day and emits a weak, growth-scaled glow at night.
    /// The glow radius is deliberately capped well below a normal outdoor lamp.
    /// </summary>
    public class CompGlowreedGlower : CompGlower
    {
        private const float ChargeSkyGlowThreshold = 0.5f;
        private const float NightSkyGlowThreshold = 0.25f;
        private const float ChargePerRareTick = 0.0125f;
        private const float DrainPerRareTick = 0.008f;

        private float storedCharge;

        public float StoredCharge => Mathf.Clamp01(storedCharge);

        public bool ShouldGlowNow
        {
            get
            {
                if (parent == null || !parent.Spawned || parent.Map == null)
                    return false;

                Plant plant = parent as Plant;
                return plant != null &&
                       plant.Growth > 0.01f &&
                       storedCharge > 0.01f &&
                       parent.Map.skyManager.CurSkyGlow <= NightSkyGlowThreshold;
            }
        }

        public float EffectiveGlowRadius
        {
            get
            {
                if (!ShouldGlowNow)
                    return 0f;

                Plant plant = parent as Plant;
                if (plant == null)
                    return 0f;

                float maximumRadius = ((CompProperties_Glower)props).glowRadius;
                return maximumRadius * Mathf.Clamp01(plant.Growth) * StoredCharge;
            }
        }

        public override void CompTickRare()
        {
            bool wasLit = ShouldGlowNow;
            float oldRadius = EffectiveGlowRadius;

            if (parent != null && parent.Spawned && parent.Map != null)
            {
                float skyGlow = parent.Map.skyManager.CurSkyGlow;
                bool outdoors = !parent.Map.roofGrid.Roofed(parent.Position);

                if (outdoors && skyGlow >= ChargeSkyGlowThreshold)
                {
                    storedCharge = Mathf.Min(1f, storedCharge + ChargePerRareTick);
                }
                else if (skyGlow <= NightSkyGlowThreshold)
                {
                    storedCharge = Mathf.Max(0f, storedCharge - DrainPerRareTick);
                }
            }

            // Let CompGlower refresh its on/off state using the updated charge.
            base.CompTickRare();

            if (parent != null && parent.Spawned && parent.Map != null)
            {
                bool isLit = ShouldGlowNow;
                float newRadius = EffectiveGlowRadius;
                if (wasLit != isLit || Mathf.Abs(oldRadius - newRadius) > 0.01f)
                {
                    parent.Map.glowGrid.MarkGlowGridDirty(parent.Position);
                }
            }
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref storedCharge, "hskGlowreedStoredCharge", 0f);

            if (Scribe.mode == LoadSaveMode.PostLoadInit)
                storedCharge = Mathf.Clamp01(storedCharge);
        }

        public override string CompInspectStringExtra()
        {
            return "HSKAlgaeCultivation_GlowreedCharge".Translate(
                Mathf.RoundToInt(StoredCharge * 100f));
        }
    }

    [HarmonyPatch(typeof(CompGlower), "get_ShouldBeLitNow")]
    internal static class CompGlower_Glowreed_ShouldBeLitNow_Patch
    {
        public static void Postfix(CompGlower __instance, ref bool __result)
        {
            if (__instance is CompGlowreedGlower glowreed)
                __result = glowreed.ShouldGlowNow;
        }
    }

    [HarmonyPatch(typeof(CompGlower), "get_GlowRadius")]
    internal static class CompGlower_Glowreed_GlowRadius_Patch
    {
        public static void Postfix(CompGlower __instance, ref float __result)
        {
            if (__instance is CompGlowreedGlower glowreed)
                __result = glowreed.EffectiveGlowRadius;
        }
    }
}

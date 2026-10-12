using UnityEngine;
using Verse;

namespace HSKAlgaeCultivation
{
    public class CompProperties_Glowreed : CompProperties_Glower
    {
        // Charge is measured as 0..1. A clear day should usually charge most
        // of the plant's reserve; a normal night should use most, but not all,
        // of a full reserve.
        public float chargePerFullDay = 3.25f;
        public float dischargePerFullNight = 1.8f;
        public float dayChargeSkyGlowThreshold = 0.2f;
        public float nightSkyGlowThreshold = 0.15f;

        public CompProperties_Glowreed()
        {
            compClass = typeof(CompGlowreed);
        }
    }

    /// <summary>
    /// Solar-charge storage and night-only light for the decorative Glowreed.
    /// CompGlower's properties are cloned per instance because GlowGrid reads
    /// glowRadius/glowColor from the CompProperties object itself.
    /// </summary>
    public class CompGlowreed : CompGlower
    {
        private const int UpdateInterval = 250;
        private const float TicksPerDay = 60000f;

        private float storedEnergy;
        private float maxGlowRadius;
        private ColorInt unscaledGlowColor = new ColorInt(32, 28, 18, 0);

        public new CompProperties_Glowreed Props => (CompProperties_Glowreed)props;

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            // ThingDef comp properties are shared by instances. Clone them so
            // each plant can have its own glow radius and color as it grows.
            CompProperties_Glowreed shared = (CompProperties_Glowreed)props;
            maxGlowRadius = shared.glowRadius;
            unscaledGlowColor = shared.glowColor;

            props = new CompProperties_Glowreed
            {
                compClass = typeof(CompGlowreed),
                glowRadius = shared.glowRadius,
                overlightRadius = shared.overlightRadius,
                glowColor = shared.glowColor,
                chargePerFullDay = shared.chargePerFullDay,
                dischargePerFullNight = shared.dischargePerFullNight,
                dayChargeSkyGlowThreshold = shared.dayChargeSkyGlowThreshold,
                nightSkyGlowThreshold = shared.nightSkyGlowThreshold
            };

            base.PostSpawnSetup(respawningAfterLoad);
            RefreshGlow(true);
        }

        public override void CompTick()
        {
            base.CompTick();

            if (!parent.Spawned || parent.Map == null || !parent.IsHashIntervalTick(UpdateInterval))
                return;

            UpdateStoredEnergy();
            RefreshGlow(false);
        }

        private void UpdateStoredEnergy()
        {
            Map map = parent.Map;
            if (map == null)
                return;

            float skyGlow = Mathf.Clamp01(map.skyManager.CurSkyGlow);
            float elapsedDays = UpdateInterval / TicksPerDay;

            // The reed only charges in open sky. Clouds and bad weather lower
            // the amount gathered because charging follows the current sky glow.
            if (!map.roofGrid.Roofed(parent.Position) &&
                skyGlow >= Props.dayChargeSkyGlowThreshold)
            {
                storedEnergy = Mathf.Min(
                    1f,
                    storedEnergy + Props.chargePerFullDay * skyGlow * elapsedDays);
            }
            else if (skyGlow <= Props.nightSkyGlowThreshold)
            {
                storedEnergy = Mathf.Max(
                    0f,
                    storedEnergy - Props.dischargePerFullNight * elapsedDays);
            }
        }

        private void RefreshGlow(bool force)
        {
            Map map = parent.Map;
            if (map == null || !parent.Spawned)
                return;

            Plant plant = parent as Plant;
            float growth = plant == null ? 0f : Mathf.Clamp01(plant.Growth);
            bool night = map.skyManager.CurSkyGlow <= Props.nightSkyGlowThreshold;
            float brightness = night ? growth * Mathf.Clamp01(storedEnergy) : 0f;

            // Keep a small nonzero radius to avoid a zero-radius glower. A
            // zero color disables the light until it is dark and charged.
            float nextRadius = brightness > 0.001f
                ? Mathf.Max(0.5f, maxGlowRadius * growth)
                : 0.5f;

            ColorInt nextColor = new ColorInt(
                Mathf.RoundToInt(unscaledGlowColor.r * brightness),
                Mathf.RoundToInt(unscaledGlowColor.g * brightness),
                Mathf.RoundToInt(unscaledGlowColor.b * brightness),
                unscaledGlowColor.a);

            CompProperties_Glowreed mutable = Props;
            bool changed = force ||
                           Mathf.Abs(mutable.glowRadius - nextRadius) > 0.01f ||
                           !mutable.glowColor.Equals(nextColor);

            if (!changed)
                return;

            mutable.glowRadius = nextRadius;
            mutable.glowColor = nextColor;
            map.glowGrid.MarkGlowGridDirty(parent.Position);
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref storedEnergy, "glowreedStoredEnergy", 0f);

            if (Scribe.mode == LoadSaveMode.PostLoadInit)
                storedEnergy = Mathf.Clamp01(storedEnergy);
        }

        public override string CompInspectStringExtra()
        {
            return "HSKAlgaeCultivation_GlowreedCharge".Translate(
                Mathf.RoundToInt(storedEnergy * 100f));
        }
    }
}

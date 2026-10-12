using RimWorld;
using UnityEngine;
using Verse;

namespace HSKAlgaeCultivation
{
    public class CompProperties_Glowreed : CompProperties_Glower
    {
        // Energy is stored as a 0..1 reserve. Charging is scaled by the
        // current outdoor sky glow; a typical clear day can refill the reserve.
        public float chargePerFullDay = 3.25f;
        public float dischargePerFullNight = 1.8f;
        public float dayChargeSkyGlowThreshold = 0.2f;

        public CompProperties_Glowreed()
        {
            compClass = typeof(CompGlowreed);
        }
    }

    /// <summary>
    /// Gives a decorative Glowreed a small solar reserve and emits light at night.
    /// The glower properties are cloned per instance so each plant can update
    /// its own radius and color as growth and stored energy change.
    /// </summary>
    public class CompGlowreed : CompGlower
    {
        private const int UpdateInterval = 250;
        private const float TicksPerDay = 60000f;

        private float storedEnergy;
        private float maxGlowRadius;
        private ColorInt unscaledGlowColor = new ColorInt(40, 180, 150, 0);

        public new CompProperties_Glowreed Props => (CompProperties_Glowreed)props;

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            CompProperties_Glowreed shared = (CompProperties_Glowreed)props;
            maxGlowRadius = shared.glowRadius;
            unscaledGlowColor = shared.glowColor;

            // Def comp properties are shared between plants; runtime glow values
            // must be private to this plant instance.
            props = new CompProperties_Glowreed
            {
                compClass = typeof(CompGlowreed),
                glowRadius = shared.glowRadius,
                overlightRadius = shared.overlightRadius,
                glowColor = shared.glowColor,
                chargePerFullDay = shared.chargePerFullDay,
                dischargePerFullNight = shared.dischargePerFullNight,
                dayChargeSkyGlowThreshold = shared.dayChargeSkyGlowThreshold
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

            float skyGlow = Mathf.Clamp01(SkyManager.CurSkyGlow);
            float elapsedDays = UpdateInterval / TicksPerDay;
            bool night = IsNight(map);

            // The plant only charges during daytime when exposed to open sky.
            // Heavy cloud cover reduces charging through the current sky-glow value.
            if (!night &&
                !map.roofGrid.Roofed(parent.Position) &&
                skyGlow >= Props.dayChargeSkyGlowThreshold)
            {
                storedEnergy = Mathf.Min(
                    1f,
                    storedEnergy + Props.chargePerFullDay * skyGlow * elapsedDays);
            }
            else if (night)
            {
                storedEnergy = Mathf.Max(
                    0f,
                    storedEnergy - Props.dischargePerFullNight * elapsedDays);
            }
        }

        private static bool IsNight(Map map)
        {
            int hour = GenLocalDate.HourOfDay(map);
            return hour < 6 || hour >= 18;
        }

        private void RefreshGlow(bool force)
        {
            Map map = parent.Map;
            if (map == null || !parent.Spawned)
                return;

            Plant plant = parent as Plant;
            float growth = plant == null ? 0f : Mathf.Clamp01(plant.Growth);
            float brightness = IsNight(map) ? growth * Mathf.Clamp01(storedEnergy) : 0f;

            // Radius and color both scale with growth; an empty reserve or
            // daytime sets the color to black so the plant does not emit light.
            float nextRadius = Mathf.Max(0.5f, maxGlowRadius * growth);
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

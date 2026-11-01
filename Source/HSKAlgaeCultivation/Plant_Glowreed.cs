using RimWorld;
using UnityEngine;
using Verse;

namespace HSKAlgaeCultivation
{
    /// <summary>
    /// Uses a texture that matches Glowreed's growth stage while preserving the
    /// HSK algae plant behavior inherited from SK.Plant_Algae.
    /// </summary>
    [StaticConstructorOnStartup]
    public class Plant_Glowreed : SK.Plant_Algae
    {
        // The young texture is used until 33% growth; the middle texture is used
        // from 33% growth until the plant is fully mature.
        private const float YoungStageEndGrowth = 0.33f;

        private const string YoungTexturePath = "Things/Plant/Glowreed/GlowreedA";
        private const string GrowingTexturePath = "Things/Plant/Glowreed/GlowreedB";

        private static readonly string[] MatureTexturePaths =
        {
            "Things/Plant/Glowreed/GlowreedC",
            "Things/Plant/Glowreed/GlowreedD",
            "Things/Plant/Glowreed/GlowreedE"
        };

        // GraphicDatabase caches graphics, so each cached Graphic_Single is reused.
        // Initialize lazily because the Def graphics are only safe to query after startup.
        private static Graphic youngGraphic;
        private static Graphic growingGraphic;
        private static readonly Graphic[] MatureGraphics = new Graphic[MatureTexturePaths.Length];
        private int matureTextureVariant = -1;

        public override Graphic Graphic
        {
            get
            {
                float growth = Mathf.Clamp01(Growth);

                if (growth < YoungStageEndGrowth)
                {
                    return GetGlowreedGraphic(ref youngGraphic, YoungTexturePath);
                }

                if (growth < 1f)
                {
                    return GetGlowreedGraphic(ref growingGraphic, GrowingTexturePath);
                }

                // Randomize the mature variant once per plant and keep it stable
                // between redraws. Seeding by thingIDNumber also makes the choice
                // reproducible after the plant is loaded from a save.
                if (matureTextureVariant < 0)
                {
                    Rand.PushState(thingIDNumber);
                    matureTextureVariant = Rand.Range(0, MatureTexturePaths.Length);
                    Rand.PopState();
                }

                return GetGlowreedGraphic(
                    ref MatureGraphics[matureTextureVariant],
                    MatureTexturePaths[matureTextureVariant]);
            }
        }

        private Graphic GetGlowreedGraphic(ref Graphic cachedGraphic, string texturePath)
        {
            if (cachedGraphic == null)
            {
                Graphic sourceGraphic = def.graphic;
                cachedGraphic = GraphicDatabase.Get<Graphic_Single>(
                    texturePath,
                    sourceGraphic.Shader,
                    sourceGraphic.drawSize,
                    sourceGraphic.color,
                    sourceGraphic.colorTwo);
            }

            return cachedGraphic;
        }
    }
}

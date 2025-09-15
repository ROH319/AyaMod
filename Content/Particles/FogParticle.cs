using AyaMod.Core;
using AyaMod.Core.Systems.ParticleSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;

namespace AyaMod.Content.Particles
{
    public class FogParticle : Particle
    {
        public override string Texture => AssetDirectory.Extras + "bulletGa000";

        public float fadeout = 0.97f;
        public float velMult = 1f;
        public static FogParticle Spawn(IEntitySource source, Vector2 center, Vector2 velocity, Color color = default(Color), float scale = 1f, float fadeout = 0.97f, float velMult = 1, float alpha = 1)
        {
            FogParticle fog = NewParticle<FogParticle>(source, center, velocity, color, scale, 0, alpha);
            fog.fadeout = fadeout;
            fog.velMult = velMult;
            return fog;
        }
        public override void OnSpawn()
        {
            Rotation = Main.rand.NextFloat(MathHelper.TwoPi);
        }
        public override void AI()
        {
            alpha *= fadeout;
            Velocity *= velMult;
            if (alpha < 0.01f) active = false;
        }
    }
}

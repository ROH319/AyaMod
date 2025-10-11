using AyaMod.Core;
using AyaMod.Core.Systems.ParticleSystem;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Renderers;

namespace AyaMod.Content.Particles
{
    public class LightSpotParticle : Particle
    {
        public override string Texture => AssetDirectory.Extras + "Ball4";


        public float scaleX = 1f;
        public float scaleY = 1f;
        public int maxTime = 60;
        public float fadeThreshold = 0.3f;
        public float scaleMultiplier = 1f;
        public static LightSpotParticle Spawn(IEntitySource source, Vector2 center, Vector2 velocity, Color color, float scale = 1f, int maxtime = 60)
        {
            LightSpotParticle particle = NewParticle<LightSpotParticle>(source, center, velocity, color, scale);
            particle.maxTime = maxtime;
            return particle;
        }
        public static LightSpotParticle[] SpawnFlare(IEntitySource source, Vector2 center, Vector2 velocity, Color color, float rotSpeed = 0.01f, int maxtime = 60, float fadethreshold = 0.3f, float scale = 1f, float scaleX = 1f, float scaleY = 1f)
        {
            LightSpotParticle[] particles = new LightSpotParticle[2];
            float startRot = Main.rand.NextFloat(MathHelper.TwoPi);
            for (int i = 0;i < 2; i++)
            {
                particles[i] = NewParticle<LightSpotParticle>(source, center, velocity, color, scale, startRot + i * MathHelper.PiOver2);
                particles[i].scaleX = scaleX * 0.1f;
                particles[i].scaleY = scaleY;
                particles[i].AngularSpeed = rotSpeed;
                particles[i].maxTime = maxtime;
                particles[i].fadeThreshold = fadethreshold;
            }
            return particles;
        }

        public override void AI()
        {

            float factor = 1f - (float)timer / maxTime;

            float fadeinFactor = Utils.Remap(factor, 1f - fadeThreshold, 1f, 1f, 0f);
            float fadeoutFactor = Utils.Remap(factor, 0f, 0f + fadeThreshold, 0f, 1f);

            //alphaMultiplier = fadeinFactor * fadeoutFactor;

            AngularSpeed *= 0.98f;



            scaleMultiplier = fadeinFactor * fadeoutFactor;

            timer++;
            if (factor < 0f) active = false;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Texture2D texture = GetTexture().Value;
            Vector2 scale = new Vector2(scaleX, scaleY) * Scale * scaleMultiplier;

            Color drawColor = color * GetAlpha();

            Main.spriteBatch.Draw(texture, Center - Main.screenPosition, null, drawColor, Rotation, texture.Size() / 2, scale, 0, 0);
            Main.spriteBatch.Draw(texture, Center - Main.screenPosition, null, drawColor, Rotation, texture.Size() / 2, scale /2f, 0, 0);

        }
    }
}

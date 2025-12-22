using AyaMod.Core;
using AyaMod.Core.Systems.ParticleSystem;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;

namespace AyaMod.Content.Particles
{
    public class StarSparkParticle : Particle
    {
        public override string Texture => AssetDirectory.StarTexturePass;


        public static StarSparkParticle Spawn(IEntitySource source, Vector2 center, Vector2 velocity, Color color, int totaltime, float scale = 1f)
        {
            StarSparkParticle particle = NewParticle<StarSparkParticle>(source, center, velocity, color);
            particle.Scale = scale;
            particle.maxtime = totaltime;
            return particle;
        }

        public override void AI()
        {
            timer += 0.5f;
            alpha *= 0.9f;
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            Texture2D texture = GetTexture().Value;
            float factor = GetTimeFactor();
            for (int i = 0; i < 4; i++)
            {
                Vector2 dir = (i * MathHelper.PiOver2 + Rotation).ToRotationVector2();
                Vector2 scale = Scale * new Vector2((1 - factor) / 2f, 1f);
                Main.spriteBatch.Draw(texture, Center + dir * factor * 20f - Main.screenPosition, null, color * alpha,
                    (i + 1) * MathHelper.PiOver2 + Rotation, new Vector2(36), scale, 0, 0);
            }

        }
    }
}

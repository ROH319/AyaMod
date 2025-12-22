using AyaMod.Core.Systems.ParticleSystem;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;

namespace AyaMod.Content.Particles
{
    public class Smoke_UnholyTwisting : Particle
    {
        public override int maxFrame => 4;

        public static Smoke_UnholyTwisting Spawn(IEntitySource source, Vector2 center, Vector2 velocity, Color color, float scale, float rot, int totaltime, float velmult)
        {
            Smoke_UnholyTwisting smoke = NewParticle<Smoke_UnholyTwisting>(source, center, velocity, color, scale, rot,maxtime: totaltime);
            smoke.SetVelMult(velmult);
            return smoke;
        }

        public override void AI()
        {
            float rot = (MathF.Sin(whoamI) * 0.5f + 0.5f) * 0.01f;
            if (whoamI % 2 == 0)
                Rotation += rot;
            else Rotation -= rot;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Texture2D texture = GetTexture().Value;

            int height = texture.Height / maxFrame;
            Rectangle rect = new Rectangle(0, frame * height, texture.Width, height);

            float scale = Scale;
            float factor = GetTimeFactor();
            float alphaFactor = Utils.Remap(factor, 0.8f, 1f, 1f, 0f) * Utils.Remap(factor, 0, 0.5f, 0f, 1f);
            float alpha = GetAlpha() * alphaFactor;
            Color drawColor = (color * alphaFactor);

            spriteBatch.Draw(texture, Center - Main.screenPosition, rect, drawColor, Rotation, rect.Size() / 2, scale, 0, 0);

        }
    }
}

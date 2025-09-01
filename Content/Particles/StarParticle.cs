using AyaMod.Core;
using AyaMod.Core.Systems.ParticleSystem;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace AyaMod.Content.Particles
{
    public class StarParticle : Particle
    {
        public override string Texture => AssetDirectory.StarTexturePass;

        public float scaleX = 1f;
        public float scaleY = 1f;
        public float Fadeout = 1f;
        public float VelMult = 1f;
        public static StarParticle Spawn(Vector2 center, Vector2 velocity, Color color, float scale, float scaleX, float scaleY, float fadeout, float velMult, float rot, float alpha)
        {
            StarParticle star = NewParticle<StarParticle>(center, velocity, color, scale, rot, alpha);
            star.scaleX = scaleX;
            star.scaleY = scaleY;
            star.Fadeout = fadeout;
            star.VelMult = velMult;
            return star;
        }

        public override void AI()
        {
            alpha *= Fadeout;
            Velocity *= VelMult;
            if (alpha < 0.01f) active = false;
            base.AI();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Texture2D texture = GetTexture().Value;
            Vector2 scale = new Vector2(scaleX, scaleY) * Scale;
            Color drawColor = color * alpha;

            Main.spriteBatch.Draw(texture, Center - Main.screenPosition, null, drawColor, Rotation + MathHelper.PiOver2, texture.Size() / 2, scale, 0, 0);

        }
    }
}

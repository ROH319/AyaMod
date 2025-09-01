using AyaMod.Core;
using AyaMod.Core.Systems.ParticleSystem;
using AyaMod.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace AyaMod.Content.Particles
{
    public class MistParticle : Particle
    {
        public override string Texture => AssetDirectory.Extras + "Mist";

        public int BloomIntensity;
        public float BloomOffset;

        public static MistParticle Spawn(Vector2 center, Vector2 velocity, Color color, float scale, float rot, float alpha, int bloomInten, float bloomOffset)
        {
            MistParticle mist = NewParticle<MistParticle>(center, velocity, color, scale, rot, alpha);
            mist.BloomIntensity = bloomInten;
            mist.BloomOffset = bloomOffset;
            return mist;
        }

        public override void AI()
        {
            base.AI();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Texture2D texture = GetTexture().Value;

            for(int i = 0;i < BloomIntensity; i++)
            {
                RenderHelper.DrawBloom(BloomIntensity, BloomOffset, texture, Center - Main.screenPosition, null, color * alpha, Rotation, texture.Size() / 2, Scale);
            }
        }
    }
}

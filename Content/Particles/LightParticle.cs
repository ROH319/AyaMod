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
using Terraria.DataStructures;
using static Terraria.GameContent.Animations.IL_Actions.Sprites;

namespace AyaMod.Content.Particles
{
    public class LightParticle : Particle
    {
        public override string Texture => AssetDirectory.Extras + "Ball";
        public int totaltime = 0;
        public float VelMult = 1f;
        public static LightParticle Spawn(IEntitySource source, Vector2 center, Vector2 velocity, Color color, int totaltime)
        {
            LightParticle light = NewParticle<LightParticle>(source, center, velocity, color);
            light.totaltime = totaltime;
            return light;
        }

        public override void AI()
        {
            Velocity *= VelMult;
            timer++;
            if (timer >= totaltime) this.active = false;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Texture2D texture = GetTexture().Value;
            float scale = 0.1f * Scale;
            float factor = timer / totaltime;
            float alphaFactor = Utils.Remap(factor, 0f, 1f, 1f, 0f);
            float alpha = this.alpha * alphaFactor;
            Color drawColor = (color * alpha).AdditiveColor();

            Main.spriteBatch.Draw(texture, Center - Main.screenPosition, null, drawColor, Rotation, texture.Size() / 2, scale, 0, 0);
            Main.spriteBatch.Draw(texture, Center - Main.screenPosition, null, drawColor, Rotation, texture.Size() / 2, scale * 0.5f, 0, 0);
            Main.spriteBatch.Draw(texture, Center - Main.screenPosition, null, drawColor, Rotation, texture.Size() / 2, scale * 0.25f, 0, 0);
        }
    }
}

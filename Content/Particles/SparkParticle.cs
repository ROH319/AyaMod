using AyaMod.Common.Easer;
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

namespace AyaMod.Content.Particles
{
    public class SparkParticle : Particle
    {
        public override string Texture => AssetDirectory.Extras + "Ball5_Alpha";

        public static SparkParticle Spawn(IEntitySource source, Vector2 center, Vector2 velocity, Color color, int totaltime, float scale = 1f)
        {
            SparkParticle particle = NewParticle<SparkParticle>(source, center, velocity, color, scale);
            particle.maxtime = totaltime;
            return particle;
        }
        public override void AI()
        {
            float factor = GetTimeFactor();

            scaleMultiplier = (MathF.Sin(Main.GameUpdateCount * 0.1f + whoamI * 17101) * 0.25f + 1f) * EaseManager.Evaluate(Ease.OutQuart, 1f -factor, 1f);
            alphaMultiplier = Utils.Remap(factor, 0.75f,1f, 1f, 0f) * Utils.Remap(factor,0f,0.25f,0f,1f);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {

            Texture2D texture = Request<Texture2D>(AssetDirectory.Extras + "Ball4_Alpha", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;

            Color drawColor = color * GetAlpha();


            Texture2D ball = Request<Texture2D>(AssetDirectory.Extras + "Ball4_Alpha", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            float ballScale = GetScale() * 64f / ball.Width * 0.5f;
            Main.spriteBatch.Draw(ball, Center - Main.screenPosition, null, drawColor.AdditiveColor() * 0.125f, Rotation, ball.Size() / 2, ballScale * 1.5f, 0, 0);
            Main.spriteBatch.Draw(ball, Center - Main.screenPosition, null, drawColor.AdditiveColor() * 1f, Rotation, ball.Size() / 2, ballScale * 0.35f, 0, 0);

            Vector2 scale = GetScale() * new Vector2(1f, 24f / texture.Width) * 0.125f;
            for(int i = 0; i < 2; i++)
            {
                Main.spriteBatch.Draw(texture, Center - Main.screenPosition, null, drawColor.AdditiveColor(), Rotation + i * MathHelper.PiOver2, texture.Size() / 2, scale, 0, 0);
            }
        }
    }
}

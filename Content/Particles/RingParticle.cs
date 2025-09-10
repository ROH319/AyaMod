using AyaMod.Common.Easer;
using AyaMod.Core.Systems.ParticleSystem;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;

namespace AyaMod.Content.Particles
{
    public class RingParticle : Particle
    {

        public float TotalTime;
        public float StartRadius;
        public float EndRadius;
        public float StartAlpha;
        public float EndAlpha;
        public Ease RadiusEaser;
        public Ease AlphaEaser;
        public int pointCount;
        public Vector2 ScaleV;
        public override void AI()
        {
            float extrascale = 2f;
            if (timer > TotalTime) active = false;
            timer++;
            base.AI();
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            float radius = MathHelper.Lerp(StartRadius, EndRadius, EaseManager.Evaluate(RadiusEaser, timer, TotalTime));
            float alpha = MathHelper.Lerp(StartAlpha, EndAlpha, EaseManager.Evaluate(AlphaEaser, timer, TotalTime));

            Texture2D texture = TextureAssets.Extra[98].Value;

            //Main.spriteBatch.End();
            //Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            for (int i = 0; i < pointCount; i++)
            {
                float factor = (float)i / pointCount;
                float dir = factor * MathHelper.TwoPi;
                Vector2 drawPos = Center + dir.ToRotationVector2() * radius * Scale - Main.screenPosition;

                Color drawColor = color * alpha * this.alpha;

                Main.spriteBatch.Draw(texture, drawPos, null, drawColor, dir, texture.Size() / 2, ScaleV, 0, 0);
                int repeat = 8;
                float offset = 1.5f * ScaleV.X;
                Color bloomcolor = drawColor * 0.3f;
                for (int j = 0; j < repeat; j++)
                {
                    for (int k = -1; k < 2; k += 2)//正向和反向
                    {
                        float fact = (float)j / repeat;
                        Vector2 posmove = dir.ToRotationVector2() * j * offset * k;
                        Main.EntitySpriteDraw(texture, drawPos + posmove, null, bloomcolor, dir, texture.Size() / 2, ScaleV, 0, 0);
                    }
                }
            }
            //Main.spriteBatch.End();
        }
        public static RingParticle Spawn(IEntitySource source, Vector2 center, Color color, float startRadius, float endRadius, float startAlpha, float endAlpha, float xScale, float yScale, int totaltime, int pointcount = 180, Ease radiusEaser = Ease.OutSine, Ease alphaEaser = Ease.InSine)
        {
            RingParticle particle = NewParticle<RingParticle>(source, center, Vector2.Zero, color, 1f);
            particle.alpha = 1f;
            particle.TotalTime = totaltime;
            particle.StartRadius = startRadius;
            particle.EndRadius = endRadius;
            particle.StartAlpha = startAlpha;
            particle.EndAlpha = endAlpha;
            particle.RadiusEaser = radiusEaser;
            particle.AlphaEaser = alphaEaser;
            particle.ScaleV = new Vector2(xScale, yScale);
            particle.pointCount = pointcount;
            return particle;
        }
    }
}

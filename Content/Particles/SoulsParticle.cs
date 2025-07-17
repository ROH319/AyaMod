using AyaMod.Common.Easer;
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
    public class SoulsParticle : Particle
    {
        public override string Texture => AssetDirectory.Extras + "Ball";
        public float Fadeout;
        public float ScaleFadeout;

        public static SoulsParticle Spawn(Vector2 center, Vector2 velocity, Color color, float fadeout = 0, float scalefade = 0, float scale = 1f, float alpha = 1f)
        {
            SoulsParticle ball = NewParticle<SoulsParticle>(center, velocity, color, scale, 0f, alpha);
            ball.Fadeout = fadeout;
            ball.ScaleFadeout = scalefade;
            return ball;
        }

        public override void AI()
        {
            if (alpha > 0f) alpha -= Fadeout;
            if (Scale > 0f) Scale -= ScaleFadeout;

            if (Scale < 0.001f) active = false;
            base.AI();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Texture2D texture = GetTexture().Value;
            Color color = this.color;
            float basebloomScale = 0.6f;
            float bloomScale = 0.4f;
            int repeat = 16;
            float maxAlpha = 0.25f;
            for (int i = 0; i < repeat; i++)
            {
                float factor = (float)i / repeat;
                Color bloomColor = Color.Lerp(Color.White, color, EaseManager.Evaluate(Ease.OutSine, factor, 1f));
                float alpha = MathHelper.Lerp(maxAlpha, 0, EaseManager.Evaluate(Ease.OutSine, factor, 1f)) * this.alpha;
                float scale = basebloomScale + MathHelper.Lerp(0, bloomScale, EaseManager.Evaluate(Ease.OutSine, factor, 1f));

                Main.spriteBatch.Draw(texture, Center - Main.screenPosition, null, bloomColor.AdditiveColor() * alpha, Rotation, texture.Size() / 2, scale * Scale, 0, 0);

            }

            //base.Draw(spriteBatch); 
        }
    }
}

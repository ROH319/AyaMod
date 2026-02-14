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
using Terraria.GameContent;
using static Terraria.GameContent.Animations.IL_Actions.Sprites;

namespace AyaMod.Content.Particles
{
    public class SoulsParticle : Particle
    {
        public override string Texture => AssetDirectory.Extras + "Ball1";
        public FloatModifier AlphaFadeout;
        public FloatModifier ScaleFadeout;
        public FloatModifier VelocityFadeout;

        public static SoulsParticle Spawn(IEntitySource source, Vector2 center, Vector2 velocity, Color color, float scale = 1f, float alpha = 1f)
        {
            SoulsParticle ball = NewParticle<SoulsParticle>(source, center, velocity, color, scale, 0f, alpha);
            return ball;
        }

        public void SetAlphaFadeout(FloatModifier alphaFadeout) {  AlphaFadeout = alphaFadeout; }
        public void SetScaleFadeout(FloatModifier scaleFadeout) {  ScaleFadeout = scaleFadeout; }
        public void SetVelFadeout(FloatModifier velFadeout) { VelocityFadeout = velFadeout; }

        public override void AI()
        {
            if (alpha > 0f) alpha = AlphaFadeout.Apply(alpha);
            if (Scale > 0f) Scale = ScaleFadeout.Apply(Scale);

            timer--;
            if (Scale < 0.001f) active = false;

            Velocity.X = VelocityFadeout.Apply(Velocity.X);
            Velocity.Y = VelocityFadeout.Apply(Velocity.Y);
            base.AI();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Texture2D texture = GetTexture().Value;/*TextureAssets.BlackTile.Value*/;
            
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

    public class SoulsParticle2 : SoulsParticle
    {
        public override string Texture => AssetDirectory.Extras + "Ball4";

        public static SoulsParticle2 Spawn(IEntitySource source, Vector2 center, Vector2 velocity, Color color, float scale = 1f, int maxtime = 60)
        {
            SoulsParticle2 particle = NewParticle<SoulsParticle2>(source, center, velocity, color, scale, maxtime: maxtime);
            return particle;
        }
        public override void AI()
        {
            float factor = GetTimeFactor();
            float fadeinFactor = EaseManager.Evaluate(Ease.OutQuad, Utils.Remap(factor, 0f, 0.3f, 0f, 1f), 1f);
            float fadeoutFactor = EaseManager.Evaluate(Ease.OutQuad, Utils.Remap(factor, 0.5f, 1f, 1f, 0f), 1f);

            alphaMultiplier = fadeinFactor * fadeoutFactor;

            float scalefadein = EaseManager.Evaluate(Ease.OutCubic, Utils.Remap(factor, 0f, 0.3f, 0f, 1f), 1f);
            float scalefadeout = EaseManager.Evaluate(Ease.OutCubic, Utils.Remap(factor, 0.5f, 1f, 1f, 0f), 1f);

            scaleMultiplier = scalefadein * scalefadeout;
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            Texture2D texture = UseAlpha ? Request<Texture2D>(Texture + "_Alpha").Value : GetTexture().Value;
            float scale = GetScale();

            Color drawColor = color * GetAlpha();

            Main.spriteBatch.Draw(texture, Center - Main.screenPosition, null, drawColor, Rotation, texture.Size() / 2, scale, 0, 0);

        }
    }
}

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
using Terraria.GameContent;

namespace AyaMod.Content.Particles
{
    public class CameraFlash : Particle
    {
        public override string Texture => AssetDirectory.EmptyTexturePass;
        public float ScaleX;
        public float ScaleY;
        public int totalTime;
        public static CameraFlash Spawn(IEntitySource source, Vector2 center, Color color, float rot, float scaleX, float scaleY, int flashTime)
        {
            CameraFlash flash = NewParticle<CameraFlash>(source, center, Vector2.Zero, color, scaleX, rot);
            flash.ScaleX = scaleX;
            flash.ScaleY = scaleY;
            flash.totalTime = flashTime;
            return flash;
        }
        public override void AI()
        {
            float factor = timer / totalTime;
            float alphaFactor = Utils.Remap(factor, 0, 1f, 1.5f, 0f);
            alpha = alphaFactor;
            Scale = Utils.Remap(factor, 0, 1f, 1.3f, 0.7f);
            timer++;

            if (factor > 1f) active = false;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(TextureAssets.BlackTile.Value, Center - Main.screenPosition, null, color * alpha, Rotation, TextureAssets.BlackTile.Value.Size() / 2, new Vector2(ScaleX, ScaleY) * Scale, 0, 0);
        }
    }

    public class CameraFlashCircle : Particle
    {
        public override string Texture => AssetDirectory.Extras + "Ball8";
        public int totalTime;
        public float radius;
        public static CameraFlashCircle Spawn(IEntitySource source, Vector2 center, Color color, float scale, int flashTime)
        {
            CameraFlashCircle flash = NewParticle<CameraFlashCircle>(source, center, Vector2.Zero, color, 1f);
            flash.totalTime = flashTime;
            flash.radius = scale;
            return flash;
        }
        public override void AI()
        {
            float factor = timer / totalTime;
            float alphaFactor = Utils.Remap(factor, 0, 1f, 1.5f, 0f);
            alpha = alphaFactor;
            Scale = Utils.Remap(factor, 0, 1f, 1.3f, 0.7f);
            timer++;

            if (factor > 1f) active = false;
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            Texture2D texture = GetTexture().Value;
            spriteBatch.Draw(texture, Center - Main.screenPosition, null, color * alpha, Rotation, texture.Size() / 2, radius / 256f * Scale, 0, 0);
        }
    }
}

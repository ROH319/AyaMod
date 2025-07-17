using AyaMod.Core.Systems.ParticleSystem;
using AyaMod.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;

namespace AyaMod.Content.Particles
{
    public class CameraFlash : Particle
    {
        public override string Texture => AssetDirectory.EmptyTexturePass;
        public float ScaleX;
        public float ScaleY;
        public int totalTime;
        public static CameraFlash Spawn(Vector2 center, Color color, float rot, float scaleX, float scaleY, int flashTime)
        {
            CameraFlash flash = NewParticle<CameraFlash>(center, Vector2.Zero, color, scaleX, rot);
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
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(TextureAssets.BlackTile.Value, Center - Main.screenPosition, null, color * alpha, Rotation, TextureAssets.BlackTile.Value.Size() / 2, new Vector2(ScaleX, ScaleY) * Scale, 0, 0);
        }
    }
}

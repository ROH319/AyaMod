using AyaMod.Common.Easer;
using AyaMod.Core;
using AyaMod.Core.Loaders;
using AyaMod.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;

namespace AyaMod.Content.Items.Cameras
{
    public class SolarCamera
    {
    }

    public class SolarShockWave : ModProjectile
    {
        public override string Texture => AssetDirectory.EmptyTexturePass;
        public ref float Radius => ref Projectile.ai[0];
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 64;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 45;
            
        }
        public override void AI()
        {
            float timeFactor = Projectile.TimeleftFactor();
            float fadein = Utils.Remap(timeFactor, 0.8f, 1f, 1f, 0f);
            float fadeout = Utils.Remap(timeFactor, 0f, 0.4f, 0f, 1f);
            Projectile.Opacity = fadein * fadeout;

            float minRange = 40f;
            float maxRange = 3200f;
            Radius = Utils.Remap(EaseManager.Evaluate(Ease.InCubic, timeFactor, 1f), 0f, 1f, maxRange, minRange);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = Request<Texture2D>(AssetDirectory.Extras + "Laser3_2", AssetRequestMode.ImmediateLoad).Value;
            Texture2D cloud = Request<Texture2D>(AssetDirectory.Extras + "GFX_clouds1_withMotes", AssetRequestMode.ImmediateLoad).Value;
            Texture2D colorMap = Request<Texture2D>(AssetDirectory.Extras + "LightYellow-Map", AssetRequestMode.ImmediateLoad).Value;
            Texture2D mask = Request<Texture2D>(AssetDirectory.Extras + "Laser2", AssetRequestMode.ImmediateLoad).Value;
            Effect shader = ShaderLoader.GetShader("Polarize");

            float alpha = Projectile.Opacity * 0.75f;
            float scale = Projectile.scale * Radius / 256f;

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            shader.Parameters["uMask"].SetValue(texture);
            shader.Parameters["uColorMap"].SetValue(colorMap);
            shader.Parameters["uCloud"].SetValue(cloud);

            shader.Parameters["preMultR"].SetValue(1.5f);
            //shader.Parameters["uTime"].SetValue((float)-Main.timeForVisualEffects * 0.002f + Projectile.whoAmI * 8);
            shader.Parameters["uTime2"].SetValue((float)-Main.timeForVisualEffects * 0.005f + Projectile.whoAmI * 8);
            //shader.Parameters["uTime2"].SetValue(25576.707f);
            shader.Parameters["maskFactor"].SetValue(0.5f);
            shader.Parameters["cloudScale"].SetValue(new Vector2(2f, 2f));
            shader.Parameters["thresholdY"].SetValue(0f);
            //shader.Parameters["flowx"].SetValue((float)(flowTime + whoamI * 3));
            shader.Parameters["multx"].SetValue(1);
            shader.CurrentTechnique.Passes[0].Apply();

            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.White.AdditiveColor() * 0.9f * alpha, (float)(Projectile.rotation/* + Main.timeForVisualEffects * 0.002f*/), texture.Size() / 2, scale, 0, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);


            return false;
        }
    }
}

using AyaMod.Common.Easer;
using AyaMod.Core;
using AyaMod.Core.Loaders;
using AyaMod.Core.Systems.ParticleSystem;
using AyaMod.Helpers;
using Microsoft.Build.Utilities;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
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
    public class StarPerishParticle : Particle
    {
        public override string Texture => AssetDirectory.EmptyTexturePass;
        public static StarPerishParticle Spawn(IEntitySource source, Vector2 center, int maxtime, float scale = 1f)
        {
            StarPerishParticle particle = NewParticle<StarPerishParticle>(source, center, Vector2.Zero, scale: scale,maxtime: maxtime);
            return particle;
        }

        public override void AI()
        {
            //Rotation += 0.02f;
            //Main.NewText($"{ParticleManager.count}");
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            float timeFactor = GetTimeFactor();
            float timeFadein = Utils.Remap(timeFactor, 0, 0.1f, 0f, 1f);
            //float timeFadeout = EaseManager.Evaluate(Ease.InCubic, Utils.Remap(timeFactor, 0.4f, 1f, 1f, 0f), 1f);
            float timeFadeout = Utils.Remap(timeFactor, 0.4f, 1f, 1f, 0f);
            float scaleFadeout = Utils.Remap(timeFactor, 0f, 1f, 1f, 1.5f);
            float yFadeout = Utils.Remap(timeFactor, 0.3f, 1f,MathHelper.Pi , MathHelper.PiOver2);

            Texture2D ball2 = Request<Texture2D>(AssetDirectory.Extras + "Ball2", AssetRequestMode.ImmediateLoad).Value;
            Main.spriteBatch.Draw(ball2, Center - Main.screenPosition, null, Color.DarkBlue.AdditiveColor() * 0.4f * timeFadein * timeFadeout, 0, ball2.Size() / 2, Scale * 8f, 0, 0);

            Texture2D flow = Request<Texture2D>(AssetDirectory.Extras + "GenFX_PlagueofMurlocs_Water_BW", AssetRequestMode.ImmediateLoad).Value;
            Texture2D cloud = Request<Texture2D>(AssetDirectory.Extras + "GFX_clouds1_sparseMotes", AssetRequestMode.ImmediateLoad).Value;
            Texture2D colorMap = Request<Texture2D>(AssetDirectory.Extras + "Blue-Map4", AssetRequestMode.ImmediateLoad).Value;
            Texture2D shape2 = Request<Texture2D>(AssetDirectory.Extras + "Laser2", AssetRequestMode.ImmediateLoad).Value;
            Vector2 scale = new Vector2(1f, 1f) * Scale * scaleFadeout;
            Effect shader = ShaderLoader.GetShader("Polarize");
            float flowDir = whoamI % 2 == 0 ? -1f : 1f;
            
            var flowTime = Main.timeForVisualEffects * 0.003f * flowDir;

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            shader.Parameters["uMask"].SetValue(shape2);
            shader.Parameters["uColorMap"].SetValue(colorMap);
            shader.Parameters["uCloud"].SetValue(cloud);
            shader.Parameters["preMultR"].SetValue(2f * timeFadein * timeFadeout);
            shader.Parameters["uTime"].SetValue((float)-Main.timeForVisualEffects * 0.002f + whoamI * 8);
            shader.Parameters["uTime2"].SetValue((float)-Main.timeForVisualEffects * 0.003f + whoamI * 8);
            //shader.Parameters["uTime2"].SetValue(25576.707f);
            shader.Parameters["maskFactor"].SetValue(0.7f);
            shader.Parameters["cloudScale"].SetValue(new Vector2(2f, 3f));
            shader.Parameters["thresholdY"].SetValue(MathF.Cos(yFadeout) * 0.25f + 0.25f);

            shader.Parameters["flowx"].SetValue((float)(flowTime + whoamI * 3));
            shader.Parameters["multx"].SetValue(2);
            shader.CurrentTechnique.Passes[0].Apply();
            Main.spriteBatch.Draw(flow, Center - Main.screenPosition, null, Color.White.AdditiveColor() * 0.9f, (float)(Rotation/* + Main.timeForVisualEffects * 0.002f*/), flow.Size() / 2, scale * 0.7f, 0, 0);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            //Texture2D colorMap = Request<Texture2D>(AssetDirectory.Extras + "Blue-Map4", AssetRequestMode.ImmediateLoad).Value;
            //Texture2D shape = Request<Texture2D>(AssetDirectory.Extras + "Ball5_1", AssetRequestMode.ImmediateLoad).Value;
            //Texture2D mask = Request<Texture2D>("Terraria/Images/Misc/Perlin").Value;/*Request<Texture2D>(AssetDirectory.Extras + "GenFX_PlagueofMurlocs_Water_BW", AssetRequestMode.ImmediateLoad).Value*/;
            //Texture2D cloud = Request<Texture2D>(AssetDirectory.Extras + "GFX_clouds1_sparseMotes", AssetRequestMode.ImmediateLoad).Value;

            //Effect effect = ShaderLoader.GetShader("Trail2");
            //Main.spriteBatch.End();
            //Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);


            //// 把变换和所需信息丢给shader
            //effect.Parameters["uNoise"].SetValue(mask);
            //effect.Parameters["uColorMap"].SetValue(colorMap);
            //effect.Parameters["uCloud"].SetValue(cloud);
            ////effect.Parameters["timer"].SetValue((float)-Main.timeForVisualEffects * 0.001f);
            //effect.Parameters["cloudScale"].SetValue(new Vector2(scaleFadeout * 2f));
            //effect.Parameters["cloudOffset"].SetValue(new Vector2(MathF.Sin(whoamI * whoamI))/*new Vector2(0.3f, (float)(Main.timeForVisualEffects * 0.002f))*/);
            //effect.Parameters["maskScale"].SetValue(scaleFadeout * 0.5f);
            //effect.Parameters["maskOffset"].SetValue(new Vector2( timeFactor / 2f,0));
            //effect.Parameters["distortIntensity"].SetValue(0.15f);
            //effect.Parameters["preMultR"].SetValue(1.5f * timeFadein * timeFadeout);
            //effect.Parameters["colorMult"].SetValue(2f);
            ////Main.graphics.GraphicsDevice.Textures[0] = colorMap;//颜色
            ////Main.graphics.GraphicsDevice.Textures[1] = shape;//形状
            ////Main.graphics.GraphicsDevice.Textures[2] = mask;//蒙版
            ////Main.graphics.GraphicsDevice.Textures[3] = cloud;//云

            ////Main.graphics.GraphicsDevice.Textures[0] = (Texture)TextureAssets.MagicPixel;
            ////Main.graphics.GraphicsDevice.Textures[1] = (Texture)TextureAssets.MagicPixel;
            ////Main.graphics.GraphicsDevice.Textures[2] = (Texture)TextureAssets.MagicPixel;
            ////Main.graphics.GraphicsDevice.Textures[3] = (Texture)TextureAssets.MagicPixel;
            ////Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
            ////Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointWrap;
            ////Main.graphics.GraphicsDevice.SamplerStates[2] = SamplerState.PointWrap;
            ////Main.graphics.GraphicsDevice.SamplerStates[3] = SamplerState.PointWrap;
            ////Main.graphics.GraphicsDevice.Textures[2] = TextureAssets.MagicPixel.Value;//形状
            //effect.CurrentTechnique.Passes[0].Apply();
            //Main.spriteBatch.Draw(shape, Center - Main.screenPosition, null, Color.White, Rotation, shape.Size() / 2, Scale * scaleFadeout, 0, 0f);

            //Main.spriteBatch.End();
            //Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            //Main.spriteBatch.End();
            //Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

        }
    }
}

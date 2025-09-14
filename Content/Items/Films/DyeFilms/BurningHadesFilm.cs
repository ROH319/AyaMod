using AyaMod.Common.Easer;
using AyaMod.Core;
using AyaMod.Core.Prefabs;
using AyaMod.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;

namespace AyaMod.Content.Items.Films.DyeFilms
{
    public class BurningHadesFilm : BaseDyeFilm
    {
        public override string Texture => AssetDirectory.Films + "CameraFilm";

        public override void OnHitNPCFilm(BaseCameraProj projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!CheckEffect()) return;
            Vector2 pos = Vector2.Lerp(projectile.Projectile.Center, target.Center, 0.5f);
            Projectile.NewProjectileDirect(projectile.Projectile.GetSource_FromAI(), pos,
                Vector2.Zero, ProjectileType<FlameingWave>(), damageDone, projectile.Projectile.knockBack, projectile.Projectile.owner);
        }
    }

    public class FlameingWave : ModProjectile
    {
        public override string Texture => AssetDirectory.EmptyTexturePass;
        public ref float Radius => ref Projectile.ai[0];
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 64;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 40;
            Projectile.penetrate = -1;
            Projectile.SetImmune(-1);
        }
        public override void OnSpawn(IEntitySource source)
        {
            Projectile.oldPos = new Vector2[1];
            Projectile.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
        }
        public override void AI()
        {
            float factor = Projectile.TimeleftFactor();
            float maxradius = 150;
            //Radius = EaseManager.Evaluate(Ease.OutCirc, 1 - factor, 1f) * maxradius;
            float threshold = 0.6f;
            if (factor > threshold)
                Radius = Utils.Remap(factor, threshold, 1f, maxradius, 30);
            else
                Radius = Utils.Remap(factor, 0f, threshold, maxradius + 80, maxradius);
            int total = (int)Radius;
            if (Projectile.oldPos.Length < total)
                Array.Resize(ref Projectile.oldPos, total);
            for(int i = 0; i < total; i++)
            {
                Vector2 pos = Projectile.Center + ((MathHelper.TwoPi + MathHelper.PiOver4 / 8) / total * i).ToRotationVector2() * Radius;
                Projectile.oldPos[i] = pos;
            }

            if (factor > 0.2f)
            {
                int dustcount = 12;
                float scalefactor = Utils.Remap(factor, 0.2f, 0.7f, 1.2f, 1.6f);
                float dustspeed = factor > threshold ? 10f : 5f;
                for (int i = 0; i < dustcount; i++)
                {
                    Vector2 pos = Projectile.Center + Main.rand.NextVector2Unit() * Radius;
                    Dust d = Dust.NewDustPerfect(pos, DustID.Torch, Projectile.Center.DirectionToSafe(pos) * dustspeed * 0.4f);
                    d.noGravity = true;
                    d.scale = scalefactor;
                }
            }
            float alphaFadein = Utils.Remap(factor, 0.8f, 1f, 1f, 0f);
            float alphaFadeout = Utils.Remap(factor, 0f, threshold, 0f, 1f);
            Projectile.Opacity = 1 * alphaFadeout * alphaFadein;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            //Texture2D mainColor = Request<Texture2D>(AssetDirectory.Extras + "White-Map-2", AssetRequestMode.ImmediateLoad).Value;
            //Texture2D mainColor = Request<Texture2D>(AssetDirectory.Extras + "Blue-Map", AssetRequestMode.ImmediateLoad).Value;
            Texture2D mainColor = Request<Texture2D>(AssetDirectory.Extras + "Orange-Red-Map2", AssetRequestMode.ImmediateLoad).Value;

            //Texture2D shape = TextureAssets.Extra[197].Value;

            Texture2D shape = Request<Texture2D>(AssetDirectory.Extras + "Laser2", AssetRequestMode.ImmediateLoad).Value;
            //Texture2D sampler = TextureAssets.Extra[193].Value;
            Texture2D sampler = Request<Texture2D>(AssetDirectory.Extras + "Airwaves_Swirlin", AssetRequestMode.ImmediateLoad).Value;
            Texture2D star = TextureAssets.Extra[98].Value;

            Effect effect = AssetDirectory.Trail;

            List<CustomVertexInfo> bars = new List<CustomVertexInfo>();

            float timeFactor = Projectile.TimeleftFactor();
            int length = Projectile.oldPos.Length;
            float mult = 4;
            Color drawColor = Color.White;
            float flamingRate = 3f;
            float radiusFadeout = Utils.Remap(timeFactor, 0f, 0.6f, 0f, 1f);
            float drawRadius = 40 * radiusFadeout;
            int total = (int)(Projectile.oldPos.Length * mult - mult);
            for (int i = 0; i < total - 1; i++)
            {
                if (Projectile.oldPos[(int)(i / mult)] == Vector2.Zero || Projectile.oldPos[(int)(i / mult) + 1] == Vector2.Zero) continue;
                float factor = i / (float)total;
                float lerpFactor = Utils.Remap(i % mult, 0, mult - 1, 1 / mult, 1f);
                Vector2 oldpos = Vector2.Lerp(Projectile.oldPos[(int)(i / mult)], Projectile.oldPos[(int)(i / mult) + 1], lerpFactor);
                float alpha = Projectile.Opacity * 0.3f;
                var normalDir = Projectile.Center.DirectionToSafe(oldpos);

                Vector2 top = oldpos + normalDir * drawRadius;
                Vector2 bottom = oldpos - normalDir * drawRadius * (1 + timeFactor);

                bars.Add(new CustomVertexInfo(top, drawColor, new Vector3(factor * flamingRate, 1 , alpha)));
                bars.Add(new CustomVertexInfo(bottom, Color.Black, new Vector3(factor * flamingRate, 0, alpha)));
            }
            if (bars.Count > 2)
            {
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, default, Main.GameViewMatrix.ZoomMatrix);
                RasterizerState originalState = Main.graphics.GraphicsDevice.RasterizerState;
                // 干掉注释掉就可以只显示三角形栅格
                //RasterizerState rasterizerState = new RasterizerState();
                //rasterizerState.CullMode = CullMode.None;
                //rasterizerState.FillMode = FillMode.WireFrame;
                //Main.graphics.GraphicsDevice.RasterizerState = rasterizerState;

                var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);//正交投影
                var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0));
                // 把变换和所需信息丢给shader
                effect.Parameters["uTransform"].SetValue(model * projection);
                //effect.Parameters["radius"].SetValue(Radius * 0.008f);
                //effect.Parameters["radius"].SetValue(Projectile.whoAmI / 1000f - Main.GameUpdateCount * 0.014f);
                effect.Parameters["timer"].SetValue((float)(Projectile.rotation));
                //Main.graphics.GraphicsDevice.Textures[0] = mainColor;//颜色
                //Main.graphics.GraphicsDevice.Textures[1] = shape;//形状
                //Main.graphics.GraphicsDevice.Textures[2] = sampler;//蒙版
                //Main.graphics.GraphicsDevice.Textures[0] = mainColor;
                //Main.graphics.GraphicsDevice.Textures[1] = sampler; 
                Main.graphics.GraphicsDevice.Textures[0] = mainColor;
                Main.graphics.GraphicsDevice.Textures[1] = shape;
                Main.graphics.GraphicsDevice.Textures[2] = sampler;
                //Main.graphics.GraphicsDevice.Textures[0] = (Texture)TextureAssets.MagicPixel;
                //Main.graphics.GraphicsDevice.Textures[1] = (Texture)TextureAssets.MagicPixel;
                //Main.graphics.GraphicsDevice.Textures[2] = (Texture)TextureAssets.MagicPixel;

                Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
                Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointWrap;
                Main.graphics.GraphicsDevice.SamplerStates[2] = SamplerState.PointWrap;

                foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    for(int i = 0;i<6;i++)
                    Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);

                }

                Main.graphics.GraphicsDevice.RasterizerState = originalState;
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            }

            return false;
        }
    }
}

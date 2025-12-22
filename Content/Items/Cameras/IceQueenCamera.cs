using AyaMod.Content.Particles;
using AyaMod.Core;
using AyaMod.Core.Loaders;
using AyaMod.Core.Prefabs;
using AyaMod.Helpers;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.ID;

namespace AyaMod.Content.Items.Cameras
{
    public class IceQueenCamera : BaseCamera
    {
        public override void SetOtherDefaults()
        {

            Item.width = 52;
            Item.height = 48;

            Item.damage = 100;

            Item.useTime = Item.useAnimation = 45;
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.shoot = ModContent.ProjectileType<IceQueenCameraProj>();
            Item.shootSpeed = 8;
            Item.knockBack = 8f;

            Item.SetShopValues(ItemRarityColor.Yellow8, Item.sellPrice(0, 5, 0, 0));
            SetCameraStats(0.07f, 182, 1.5f);
            SetCaptureStats(1000, 60);
        }
    }
    public class IceQueenCameraProj : BaseCameraProj
    {
        public override Color outerFrameColor => new Color(53, 220, 255);
        public override Color innerFrameColor => new Color(211, 158, 255);
        public override Color focusCenterColor => base.focusCenterColor;
        public override Color flashColor => new Color(243, 209, 255).AdditiveColor() * 0.5f;

        public override void OnSnapInSight()
        {
            if (!Projectile.MyClient()) return;

            int timeleft = 45;
            float maxRadius = 150;
            Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center,
                Vector2.Zero, ProjectileType<FrostWave>(), Projectile.damage, Projectile.knockBack, Projectile.owner, timeleft,0, maxRadius);
        }

        public override void PostAI()
        {
            if(!player.ItemTimeIsZero && player.itemTime % 2 == 0)
            {
                int dustcount = 8;
                for (int i = 0; i < dustcount; i++)
                {
                    float range = Main.rand.Next(75, 100) * 2;
                    var pos = Projectile.Center + Main.rand.NextVector2Unit() * range;
                    var vel = pos.DirectionToSafe(Projectile.Center).RotatedBy(MathHelper.PiOver4/2) * range / 12;
                    FogParticle.Spawn(Projectile.GetSource_FromAI(), pos, vel, new Color(106, 152, 216).AdditiveColor() * 0.1f, fadeout: 0.96f, velMult: 0.94f);
                }
            }
        }
    }

    public class FrostWave : ModProjectile
    {
        public override string Texture => AssetDirectory.EmptyTexturePass;
        public ref float Radius => ref Projectile.ai[1];
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 64;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.SetImmune(-1);
            Projectile.timeLeft = 40;
        }

        public override void OnSpawn(IEntitySource source)
        {
            if (Projectile.ai[0] != 0)
                Projectile.timeLeft = (int)Projectile.ai[0];
            Projectile.oldPos = new Vector2[1];
            Projectile.rotation = Main.rand.NextFloat(MathHelper.TwoPi);

            float starscale = Projectile.ai[2] / 300;
            for (int i = 0; i < 64; i++)
            {
                Vector2 pos = Projectile.Center/* + Main.rand.NextVector2Unit() * Main.rand.NextFloat(20)*/;
                float starspeed = Main.rand.NextFloat(2, 7) * Projectile.ai[2] / 75f;
                Vector2 vel = Main.rand.NextVector2Unit() * starspeed;
                StarParticle.Spawn(source, pos, vel, Color.LightSkyBlue.AdditiveColor(),
                    starscale, 0.4f, 1.2f, 0.85f, 0.93f, vel.ToRotation(), 1f);
            }

            //SoundEngine.PlaySound(SoundID.Item120, Projectile.Center);
            //int fogcount = 64;
            //float fogscale = Projectile.ai[2] / 300;

            //float fogspeed = Main.rand.NextFloat(5, 7) * Projectile.ai[2] / 75f;
            //for(int i = 0; i < fogcount; i++)
            //{
            //    Vector2 pos = Projectile.Center + Main.rand.NextVector2Unit() * Main.rand.NextFloat(30, 60);
            //    Vector2 vel = Main.rand.NextVector2Unit() * fogspeed;
            //    FogParticle.Spawn(source, pos, vel, new Color(106, 152, 216).AdditiveColor(), fogscale,
            //        0.94f, 0.94f, 1f);

            //}
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return base.Colliding(projHitbox, targetHitbox);
        }

        public override void AI()
        {
            float factor = Projectile.TimeleftFactor();
            float maxradius = 150;
            if (Projectile.ai[2] != 0) maxradius = Projectile.ai[2];
            float threshold = 0.75f;
            if (factor > threshold)
                Radius = Utils.Remap(factor, threshold, 1f, maxradius, 30);
            else
                Radius = Utils.Remap(factor, 0f, threshold, maxradius * 1.7f, maxradius);

            int total = (int)Radius * 2;
            if (Projectile.oldPos.Length < total)
                Array.Resize(ref Projectile.oldPos, total);
            for (int i = 0; i < total; i++)
            {
                Vector2 pos = Projectile.Center + ((MathHelper.TwoPi + MathHelper.PiOver4 / 24) / total * i + Projectile.rotation).ToRotationVector2() * Radius;
                Projectile.oldPos[i] = pos;
            }
            if (factor > 0.2f)
            {
                int dustcount = 8;
                float scalefactor = Utils.Remap(factor, 0.2f, 0.7f, 1.2f, 1.6f);
                float dustspeed = factor > threshold ? 10f : 4f;
                for (int i = 0; i < dustcount; i++)
                {
                    Vector2 pos = Projectile.Center + Main.rand.NextVector2Unit() * Radius;
                    //67
                    Dust d = Dust.NewDustPerfect(pos, 92, Projectile.Center.DirectionToSafe(pos) * dustspeed * 0.4f);
                    d.noGravity = true;
                    d.scale = scalefactor;
                }

                int fogcount = 8;
                float fogspeed = factor > threshold ? 10f : 4f;
                for(int i = 0; i < fogcount; i++)
                {
                    Vector2 pos = Projectile.Center + Main.rand.NextVector2Unit() * Radius;
                    FogParticle.Spawn(Projectile.GetSource_FromAI(), pos, Projectile.Center.DirectionToSafe(pos) * dustspeed * 0.4f,
                        new Color(106, 152, 216).AdditiveColor() * 0.1f, 1.5f, 0.94f, 0.93f, 1f);
                }
            }

            float alphaFadeout = Utils.Remap(factor, 0f, threshold, 0f, 1f);
            Projectile.Opacity = 1 * alphaFadeout;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainColor = Request<Texture2D>(AssetDirectory.Extras + "Ice-Map3", AssetRequestMode.ImmediateLoad).Value;

            Texture2D shape = Request<Texture2D>(AssetDirectory.Extras + "Laser3", AssetRequestMode.ImmediateLoad).Value;
            //Texture2D sampler = Request<Texture2D>(AssetDirectory.Extras + "DRGFX_SpinRing1_Broken_Sharper", AssetRequestMode.ImmediateLoad).Value;
            Effect effect = ShaderLoader.GetShader("Trail");
            List<CustomVertexInfo> bars = new List<CustomVertexInfo>();

            float timeFactor = Projectile.TimeleftFactor();
            int length = Projectile.oldPos.Length;
            float mult = 4;
            Color drawColor = Color.White;
            float flamingRate = 3f;
            float radiusFadeout = Utils.Remap(timeFactor, 0f, 0.7f, 0f, 1f);
            float width = MathF.Min(Radius / 3, Projectile.ai[2] / 3);
            float drawRadius = (width) * radiusFadeout;
            int total = (int)(Projectile.oldPos.Length * mult - mult);
            for (int i = 0; i < total - 1; i++)
            {
                if (Projectile.oldPos[(int)(i / mult)] == Vector2.Zero || Projectile.oldPos[(int)(i / mult) + 1] == Vector2.Zero) continue;
                float factor = i / (float)total;
                float lerpFactor = Utils.Remap(i % mult, 0, mult - 1, 1 / mult, 1f);
                Vector2 oldpos = Vector2.Lerp(Projectile.oldPos[(int)(i / mult)], Projectile.oldPos[(int)(i / mult) + 1], lerpFactor);
                float alpha = Projectile.Opacity * 0.25f * (0.5f + (150f / Projectile.ai[2]) * 0.5f);
                var normalDir = Projectile.Center.DirectionToSafe(oldpos);

                Vector2 top = oldpos + normalDir * drawRadius * 0.5f;
                Vector2 bottom = oldpos - normalDir * drawRadius * (1 + timeFactor);

                bars.Add(new CustomVertexInfo(top, drawColor, new Vector3(factor * flamingRate, 0, alpha)));
                bars.Add(new CustomVertexInfo(bottom, Color.Black, new Vector3(factor * flamingRate, 1, alpha)));
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
                //Main.graphics.GraphicsDevice.Textures[2] = sampler;
                //Main.graphics.GraphicsDevice.Textures[0] = (Texture)TextureAssets.MagicPixel;
                //Main.graphics.GraphicsDevice.Textures[1] = (Texture)TextureAssets.MagicPixel;
                //Main.graphics.GraphicsDevice.Textures[2] = sampler;
                Main.graphics.GraphicsDevice.Textures[2] = (Texture)TextureAssets.MagicPixel;

                Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
                Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointWrap;
                Main.graphics.GraphicsDevice.SamplerStates[2] = SamplerState.PointWrap;

                foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    for (int i = 0; i <5; i++)
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

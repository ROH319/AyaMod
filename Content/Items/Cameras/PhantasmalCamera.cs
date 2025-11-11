using AyaMod.Common.Easer;
using AyaMod.Content.Particles;
using AyaMod.Core;
using AyaMod.Core.Configs;
using AyaMod.Core.ModPlayers;
using AyaMod.Core.Prefabs;
using AyaMod.Core.Systems.Trails;
using AyaMod.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.ID;

namespace AyaMod.Content.Items.Cameras
{
    public class PhantasmalCamera : BaseCamera
    {
        public override void SetOtherDefaults()
        {

            Item.width = 52;
            Item.height = 48;

            Item.damage = 480;

            Item.useTime = Item.useAnimation = 50;
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.shoot = ModContent.ProjectileType<PhantasmalCameraProj>();
            Item.shootSpeed = 8;
            Item.knockBack = 8f;

            Item.SetShopValues(ItemRarityColor.StrongRed10, Item.sellPrice(0, 5, 0, 0));
            SetCameraStats(0.08f, 194, 1.5f);
            SetCaptureStats(1000, 60);
        }
    }

    public class PhantasmalCameraProj : BaseCameraProj
    {
        public override Color outerFrameColor => new Color(34, 221, 151);
        public override Color innerFrameColor => new Color(167, 245, 227);
        public override Color focusCenterColor => base.focusCenterColor;
        public override Color flashColor => new Color(200, 244, 234).AdditiveColor() * 0.5f;

        public static int TrailInterval => 3;

        public override void SetStaticDefaults()
        {
            Projectile.SetTrail(2, 60);
        }
        public override void SetOtherDefault()
        {
            //CanSpawnFlash = false;
        }

        public override void OnSpawn(IEntitySource source)
        {
            int type = ProjectileType<PhantasmalFrame>();
            for(int i = 0; i < 6; i++)
            {

                Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, type, Projectile.damage, Projectile.knockBack, player.whoAmI, Projectile.whoAmI, i);
            }
        }

        public override void MoveMent(CameraPlayer mplr)
        {
            base.MoveMent(mplr);
            //float slowedchase = CameraStats.ChaseFactor;
            ////if (mplr.Player.itemTime != 0) slowedchase *= CameraStats.SlowFactor;
            //Vector2 previous = Projectile.Center;
            //Projectile.Center = Vector2.Lerp(Projectile.Center, mplr.MouseWorld, slowedchase);
            //ComputedVelocity = Projectile.Center - previous;
            //Projectile.rotation = Utils.AngleTowards(Projectile.rotation, Projectile.AngleToSafe(mplr.MouseWorld), 0.1f);
        }
        public override void OnSnapInSight()
        {
            foreach(var projectile in Main.ActiveProjectiles)
            {
                if (projectile.type != ProjectileType<PhantasmalFrame>() || projectile.ai[2] >= 0) continue;

                projectile.ai[2] = 5 + projectile.ai[1] * 4;
            }

            if (!Projectile.MyClient()) return;

            int eyecount = 3;

            int type = ProjectileType<PhantasmalEye>();
            int damage = (int)(Projectile.damage * 0.3f);
            for(int i = 0;i < eyecount; i++)
            {
                Vector2 vel = Main.rand.NextVector2Unit() * Main.rand.NextFloat(8, 12);

                Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, vel, type, damage, Projectile.knockBack, Projectile.owner);
            }
        }

        public override void PostDraw(Color lightColor)
        {
            float width = Size * 1.4f / 2;
            
            for(int i = 0; i < Projectile.oldPos.Length - 2; i++)
            {
                if (Projectile.oldPos[i] == Vector2.Zero || Projectile.oldPos[i + 1] == Vector2.Zero) continue;
                float trailFactor = (float)i / (Projectile.oldPos.Length - 2);
                float trailFactornext = (float)(i + 1) / (Projectile.oldPos.Length - 2);
                float alphaFactor = Utils.Remap(trailFactor, 0f, 1f, 0.5f, 0.1f);
                float min = 0.16f;
                float widthFactor = Utils.Remap(trailFactor, min, 1f, 0.9f, 0.4f);
                float widthFactornext = Utils.Remap(trailFactornext, min, 1f, 0.9f, 0.4f);
                float trailwidth = width * widthFactor;
                float trailwidthnext = width * widthFactornext;
                for (int j = -1; j < 2; j += 2)
                {
                    Vector2 dir = Projectile.oldRot[i].ToRotationVector2().RotatedBy(MathHelper.PiOver2 * j);
                    Vector2 dirnext = Projectile.oldRot[i + 1].ToRotationVector2().RotatedBy(MathHelper.PiOver2 * j);


                    Vector2 offset = Projectile.oldRot[i].ToRotationVector2() * Size / 2 * widthFactor;
                    Vector2 offsetnext = Projectile.oldRot[i + 1].ToRotationVector2() * Size / 2 * widthFactornext;
                    Vector2 pos = Projectile.oldPos[i] + Projectile.Size / 2 + dir * trailwidth + offset;
                    Vector2 next = Projectile.oldPos[i + 1] + Projectile.Size / 2 + dirnext * trailwidthnext + offsetnext;
                    //Utils.DrawLine(Main.spriteBatch, pos, next, outerFrameColor * alphaFactor, outerFrameColor * alphaFactor, 2f);
                }
            }
        }
    }

    public class PhantasmalFrame : ModProjectile
    {
        public override string Texture => AssetDirectory.EmptyTexturePass;
        public ref float Owner => ref Projectile.ai[0];
        public ref float Offset => ref Projectile.ai[1];
        public ref float FocusTimeleft => ref Projectile.ai[2];
        public float Size;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 1;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true; 
            Projectile.DamageType = ReporterDamage.Instance;
            Projectile.timeLeft = 2;
            Projectile.penetrate = -1;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            var offset = Projectile.rotation.ToRotationVector2() * Size * 0.5f;
            float point = 0;
            float height = Size * 1.4f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center - offset, Projectile.Center + offset, height, ref point);
        }
        public override bool? CanDamage()
        {
            return FocusTimeleft == 0;
        }
        public override void AI()
        {
            Projectile camera = Main.projectile[(int)Owner];
            if (camera.TypeAlive(ProjectileType<PhantasmalCameraProj>()))
            {
                Projectile.timeLeft++;

                int trailIndex = PhantasmalCameraProj.TrailInterval - 1 + (int)((Offset) * PhantasmalCameraProj.TrailInterval);
                if (camera.oldPos[trailIndex] != Vector2.Zero)
                    Projectile.Center = camera.oldPos[trailIndex] + camera.Size / 2;
                Projectile.rotation = camera.oldRot[trailIndex];

                var mcamera = camera.ModProjectile as PhantasmalCameraProj;
                float sizeFactor = Utils.Remap(Offset, 0, 5, 0.9f, 0.7f);
                Size = mcamera.Size * sizeFactor;
                Projectile.Opacity = Utils.Remap(mcamera.ComputedVelocity.Length(), 3f, 20f, 0f, 1f);
                if(FocusTimeleft > -1)
                {
                    FocusTimeleft--;
                }

                var player = (camera.ModProjectile as BaseCameraProj).player;
                if (player.AliveCheck(Projectile.Center, 2000))
                {

                    if(FocusTimeleft == 1)
                    {
                        Helper.PlayPitched("Snap", ClientConfig.Instance.SnapVolume * 0.5f, position: player.Center);

                        Color flashColor = new Color(200, 244, 234).AdditiveColor() * 0.5f * ClientConfig.Instance.SnapFlashAlpha;
                        CameraFlash.Spawn(null, Projectile.Center, flashColor, Projectile.rotation, Size * 0.9f / 16f, Size * 1.4f * 0.9f / 16f, 16);

                    }
                }
            }
            else Projectile.Kill();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            float alphaFactor = Utils.Remap(Offset, 0, 6, 0.8f, 0.2f);/*0.8f*/;
            alphaFactor *= 1 + MathF.Cos(Main.GameUpdateCount * 0.2f + Offset * 1.2f) * 0.6f;
            float sizex = Size;
            float sizey = Size * 1.4f;

            var pos = AyaUtils.GetCameraRect(Projectile.Center, Projectile.rotation, sizex, sizey);

            Color borderColor = new Color(70, 255, 193).AdditiveColor() * Projectile.Opacity * alphaFactor;
            RenderHelper.DrawCameraFrame(Main.spriteBatch, pos, borderColor, 2f, 0.2f);

            Vector2 dir = pos[0].DirectionToSafe(pos[2]);
            Vector2 ndir = pos[0].DirectionToSafe(pos[1]);

            Color centerColor = new Color(24,93,66) * Projectile.Opacity * alphaFactor * 0.9f;
            //焦点
            Utils.DrawLine(Main.spriteBatch, Projectile.Center - dir * sizex / 8f, Projectile.Center + dir * sizex / 8f, centerColor, centerColor, 2f);
            Utils.DrawLine(Main.spriteBatch, Projectile.Center - ndir * sizey / 8f, Projectile.Center + ndir * sizey / 8f, centerColor, centerColor, 2f);


            return false;
        }
    }

    public class PhantasmalEye : ModProjectile
    {
        public override string Texture => AssetDirectory.Projectiles + Name;
        public static MultedTrail strip = new MultedTrail();

        public override void SetStaticDefaults()
        {
            Projectile.SetTrail(2, 25);
            ProjectileID.Sets.CultistIsResistantTo[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 28;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.SetImmune(10);
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            if (!Projectile.Chase(2000, 28, 0.015f))
                Projectile.velocity += Projectile.velocity.Length(0.06f);
            Projectile.rotation = Projectile.velocity.ToRotation();

            {
                Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, 323,Scale:0.8f);
                d.noGravity = true;
            }
        }
        public Color ColorFunction(float progress)
        {
            Color drawColor = new Color(34, 221, 151);
            float div = 0.2f;
            if (progress < 0.2f)
                return Color.Lerp(Color.White, drawColor, Utils.Remap(progress, 0, div, 0f, 1f)).AdditiveColor() * Projectile.Opacity;
            float factor = Utils.Remap(progress, div, 1f, 0f, 1f);
            return Color.Lerp(drawColor, Color.Black, factor).AdditiveColor() * Projectile.Opacity;
        }
        public float WidthFunction(float progress) => 20f;
        public float AlphaFunction(float progress)
        {
            float fadeinFactor = Utils.Remap(progress, 0f, 0.1f, 0f, 1f);
            return EaseManager.Evaluate(Ease.InOutSine, 1f - progress, 1f) * Projectile.Opacity * 2f * fadeinFactor;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;

            Texture2D mainColor = Request<Texture2D>(AssetDirectory.Extras + "Cyan-Map", AssetRequestMode.ImmediateLoad).Value;
            Texture2D shape = TextureAssets.Extra[197].Value;
            Texture2D sampler = TextureAssets.Extra[189].Value;

            Effect effect = AssetDirectory.Trail;

            List<CustomVertexInfo> bars = new List<CustomVertexInfo>();

            float width = 20 * Projectile.scale;

            for(int i = 0;i<Projectile.oldPos.Length - 2; i++)
            {
                if (Projectile.oldPos[i] == Vector2.Zero || Projectile.oldPos[i+1] == Vector2.Zero) continue;

                var normal = Projectile.oldPos[i + 1] - Projectile.oldPos[i];
                normal = normal.RotatedBy(MathHelper.PiOver2).SafeNormalize(Vector2.Zero);

                var factor = 1 - i / (float)Projectile.oldPos.Length;
                Color color = Color.Lerp(Color.Black,Color.White,  factor);
                var alpha = EaseManager.Evaluate(Ease.InSine, factor, 1f) * 1.05f * Projectile.Opacity;
                Vector2 top = Projectile.oldPos[i] + Projectile.Size / 2 + normal * width;
                Vector2 bottom = Projectile.oldPos[i] + Projectile.Size / 2 - normal * width;
                bars.Add(new(top, color, new Vector3((float)Math.Sqrt(factor) * 2.5f, 1, alpha)));
                bars.Add(new(bottom, color, new Vector3((float)Math.Sqrt(factor) * 2.5f, 0, alpha)));

            }

            if(bars.Count > 2)
            {
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                RasterizerState originalState = Main.graphics.GraphicsDevice.RasterizerState;

                var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);//正交投影
                var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0));
                // 把变换和所需信息丢给shader
                effect.Parameters["uTransform"].SetValue(model * projection);
                effect.Parameters["timer"].SetValue((float)Main.timeForVisualEffects * 0.025f);
                Main.graphics.GraphicsDevice.Textures[0] = mainColor;//颜色
                Main.graphics.GraphicsDevice.Textures[1] = shape;//形状
                Main.graphics.GraphicsDevice.Textures[2] = sampler;//蒙版
                Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
                Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointWrap;
                Main.graphics.GraphicsDevice.SamplerStates[2] = SamplerState.PointWrap;

                foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);

                }

                Main.graphics.GraphicsDevice.RasterizerState = originalState;
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            }

            strip.PrepareStrip(Projectile.oldPos, 3, ColorFunction, WidthFunction,
                Projectile.Size / 2 - Main.screenPosition, AlphaFunction);
            Main.graphics.GraphicsDevice.Textures[0] = shape;
            strip.DrawTrail();

            RenderHelper.DrawBloom(6, 4, texture, Projectile.Center - Main.screenPosition, null, Color.White.AdditiveColor() * 0.2f, Projectile.rotation, texture.Size() / 2, Projectile.scale * 0.9f);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, texture.Size() / 2, Projectile.scale * 0.9f, 0);
            return false;
        }
    }
}

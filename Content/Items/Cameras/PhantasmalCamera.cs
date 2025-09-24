using AyaMod.Content.Particles;
using AyaMod.Core;
using AyaMod.Core.Configs;
using AyaMod.Core.ModPlayers;
using AyaMod.Core.Prefabs;
using AyaMod.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
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
            SetCaptureStats(100, 5);
        }
    }

    public class PhantasmalCameraProj : BaseCameraProj
    {
        public override Color outerFrameColor => new Color(34, 221, 151);
        public override Color innerFrameColor => new Color(167, 245, 227);
        public override Color focusCenterColor => base.focusCenterColor;
        public override Color flashColor => new Color(200, 244, 234).AdditiveColor() * 0.5f;

        public static int TrailInterval => 10;

        public override void SetStaticDefaults()
        {
            Projectile.SetTrail(2, 60);
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
            float slowedchase = CameraStats.ChaseFactor;
            //if (mplr.Player.itemTime != 0) slowedchase *= CameraStats.SlowFactor;
            Vector2 previous = Projectile.Center;
            Projectile.Center = Vector2.Lerp(Projectile.Center, mplr.MouseWorld, slowedchase);
            ComputedVelocity = Projectile.Center - previous;
            Projectile.rotation = Utils.AngleTowards(Projectile.rotation, Projectile.AngleToSafe(mplr.MouseWorld), 0.1f);
        }
        public override void OnSnapInSight()
        {
            foreach(var projectile in Main.ActiveProjectiles)
            {
                if (projectile.type != ProjectileType<PhantasmalFrame>() || projectile.ai[2] >= 0) continue;

                projectile.ai[2] = 5 + projectile.ai[1] * 4;
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
                float sizeFactor = Utils.Remap(Offset, 0, 5, 0.9f, 0.4f);
                Size = mcamera.Size * sizeFactor;
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
}

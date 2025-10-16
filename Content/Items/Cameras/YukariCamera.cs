using AyaMod.Core.Prefabs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Enums;
using Terraria.ID;
using Terraria;
using Terraria.DataStructures;
using AyaMod.Helpers;
using AyaMod.Core;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using Terraria.Audio;

namespace AyaMod.Content.Items.Cameras
{
    public class YukariCamera : BaseCamera
    {
        public override void SetOtherDefaults()
        {
            Item.width = 52;
            Item.height = 48;

            Item.damage = 80;

            Item.useTime = Item.useAnimation = 50;
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.shoot = ProjectileType<YukariCameraProj>();
            Item.shootSpeed = 8;
            Item.knockBack = 9f;

            Item.SetShopValues(ItemRarityColor.LightRed4, Item.sellPrice(0, 0, 60, 0));
            SetCameraStats(0.05f, 126, 1.5f, 0.5f);
            SetCaptureStats(1000, 60);
        }

    }


    public class YukariCameraProj : BaseCameraProj
    {
        public override Color outerFrameColor => new Color(172, 124, 197);
        public override Color innerFrameColor => new Color(242, 58, 48) * 0.7f;
        public override Color focusCenterColor => base.focusCenterColor;
        public override Color flashColor => new Color(234, 203, 241).AdditiveColor() * 0.5f;
        public override void OnSpawn(IEntitySource source)
        {
            Vector2 pos = player.Center/*AyaUtils.RandAngle.ToRotationVector2() * 400 + player.GetModPlayer<CameraPlayer>().MouseWorld*/;
            Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), pos, Vector2.Zero, ProjectileType<YinYangBall>(), Projectile.damage, 12, Projectile.owner,Projectile.whoAmI);
        }
        public override void OnSnapProjectile(Projectile projectile)
        {
            if (projectile.type != ProjectileType<YinYangBall>()) return;
            if (projectile.localAI[2] < 1)
            {
                projectile.localAI[0] = -1;
                projectile.localAI[2] = 6f;
                Vector2 dir = Projectile.DirectionToSafe(projectile.Center);
                projectile.velocity = dir * 18f;
                Projectile.localAI[2] = 1;
            }

        }
        public override void PostSnap()
        {
            if (Projectile.localAI[2] < 1)
            {
                float baserot = AyaUtils.RandAngle;
                for(int i = 0; i < 10; i++)
                {
                    float rot = baserot + MathHelper.TwoPi / 10 * i;
                    Vector2 dir = rot.ToRotationVector2();
                    Vector2 vel = dir * 20;
                    var p = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, vel, ModContent.ProjectileType<Needle>(), (int)(Projectile.damage * 0.4f), Projectile.knockBack / 4, Projectile.owner);
                    p.rotation = rot;
                }

            }
            else Projectile.localAI[2] = 0;
        }
    }

    public class YinYangBall : ModProjectile
    {
        public override string Texture => AssetDirectory.Projectiles + Name;
        public ref float Owner => ref Projectile.ai[0];
        public ref float MissingOwner => ref Projectile.ai[1];
        public ref float TargetNext => ref Projectile.localAI[0];
        public ref float TargetPrev => ref Projectile.localAI[1];
        public ref float CollisionLeft => ref Projectile.localAI[2];
        public override void SetStaticDefaults()
        {
            Projectile.SetTrail(2, 18);
        }
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 45;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.SetImmune(15);
            Projectile.scale = 1.5f;
            Projectile.timeLeft = 99999999;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            var prev = CollisionLeft;

            TargetPrev = target.whoAmI;
            CollisionLeft--;

            var othertarget = Projectile.FindCloestNPCIgnoreIndex(800, false, !Projectile.tileCollide, (int)TargetPrev);
            if (othertarget == null)
            {
                TargetPrev = -1;
                CollisionLeft = 0;
            }
            else TargetNext = othertarget.whoAmI;

            if (CollisionLeft == 0 && Projectile.velocity.Length() > 10)
            {
                Projectile.velocity = Projectile.velocity.Length(10);
            }

            //if (CollisionLeft > 0)
            {
                float knockBackFactor = target.knockBackResist;
                if (target.life <= 0) knockBackFactor = 1;
                Vector2 totarget = Projectile.Center.DirectionToSafe(target.Center);
                if (CollisionLeft == 0 && prev > 0) totarget = totarget.RotatedBy(MathHelper.PiOver4 * (Main.rand.NextBool() ? 1 : -1));
                Vector2 vec = (-Projectile.velocity).Reflect(totarget.RotatedBy(MathHelper.PiOver2));
                Projectile.velocity = Vector2.Lerp(vec, Projectile.velocity, knockBackFactor);


                if (Projectile.velocity.Length() < 4f) Projectile.velocity = Projectile.velocity.Length(4);
                if (Projectile.velocity.Length() > 5f) CreateWaveDust(Projectile.Center + totarget * 22f, totarget.ToRotation());
            }
            //Projectile.velocity *= -1;
            //Projectile.velocity = Projectile.velocity.RotatedBy(dir * MathHelper.PiOver2);


            if(prev > 0)
            {
                //89,88,74,73
                SoundEngine.PlaySound(SoundID.Item70 with {  Pitch = 0.5f, Volume = 1f, MaxInstances = 20}, Projectile.Center);
            }
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            float dir = oldVelocity.ToRotation();

            Projectile.BounceOverTile(oldVelocity);
            float angle = Projectile.velocity.AngleBetween(oldVelocity);
            if (angle >= MathHelper.PiOver4)
            {

                if (oldVelocity.Length() > 2f) CreateWaveDust(Projectile.Center + dir.ToRotationVector2() * 22f, dir);
                Projectile.velocity *= .8f;
            }

            TargetPrev = -1;
            
            return false;
        }
        public void CreateWaveDust(Vector2 center, float rot)
        {

            int dustamount = 48;
            for(int i = 0; i < dustamount; i++)
            {
                int dustType = Main.rand.NextBool() ? DustID.RedTorch : DustID.WhiteTorch;
                float dir = MathHelper.TwoPi / dustamount * i;
                Vector2 vec = dir.ToRotationVector2();
                float scale = Utils.Remap(MathF.Abs(vec.X), 0f, 1f, 1f, 0.5f);
                vec.X /= 2.5f;
                vec = vec.RotatedBy(rot);
                Dust d = Dust.NewDustPerfect(center + vec * 10f, dustType, vec * 2.2f,Scale:scale * 5f);
                d.noGravity = true;
            }
        }
        public override void OnSpawn(IEntitySource source)
        {
            Projectile.Opacity = 0f;
            TargetPrev = -1;
        }

        public override void AI()
        {
            Projectile.timeLeft++;

            Projectile camera = Main.projectile[(int)Owner];
            if (camera.TypeAlive(ProjectileType<YukariCameraProj>()) && MissingOwner >= 0)
            {
                if (Projectile.Opacity < 1f)
                    Projectile.Opacity += 0.01f;

                if (CollisionLeft < 1)
                {
                    Vector2 targetVel = Projectile.DirectionToSafe(camera.Center) * 20f;
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, targetVel, 0.01f);
                }
                else
                {
                    //if (Projectile.velocity.Length() < 4f) Projectile.localAI[2] = 0;
                    float range = 800;
                    NPC target = TargetNext < 0 ? null : Main.npc[(int)TargetNext];
                    if (target == null || !target.CanBeChasedBy() || TargetNext == TargetPrev)
                    {
                        target = Projectile.FindCloestNPCIgnoreIndex(range, false, !Projectile.tileCollide, (int)TargetPrev);
                        if (target != null)
                        {
                            TargetNext = target.whoAmI;
                        }
                    }

                    if (target != null)
                    {
                        float speed = Utils.Remap(CollisionLeft / 6f,0f,1f, 14f, 28f);
                        float speedFactor = Utils.Remap(Projectile.Distance(target.Center), 500, 1000, 1.3f, 2.8f);
                        
                        Projectile.velocity = Projectile.DirectionToSafe(target.Center) * speed;
                        //float chaseFactor = Utils.Remap(Projectile.Distance(target.Center) / 600, 0f, 1f, 0.03f, 0.07f);
                        //Projectile.Chase(target, 25, chaseFactor * 1.2f);
                    }
                    else
                    {
                        TargetPrev = -1;
                        CollisionLeft = 0;
                        if (Projectile.velocity.Length() > 12) Projectile.velocity = Projectile.velocity.Length(12);
                    }
                }
            }
            else
            {
                MissingOwner = -1;
                Projectile.Opacity -= 0.05f;
                if (Projectile.Opacity < 0.1f) Projectile.Kill();

            }
            Projectile.rotation += 0.015f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Texture2D whitePart = Request<Texture2D>(AssetDirectory.Projectiles + "YinYangBall2",ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            Texture2D colorPart = Request<Texture2D>(AssetDirectory.Projectiles + "YinYangBall1",ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            Vector2 origin = texture.Size() / 2;
            float alpha = Projectile.Opacity;

            float chaseFactor = MathHelper.Clamp(CollisionLeft / 12f, 0f, 1f);
            Color whiteColor = Color.White * alpha * 0.7f;
            Color color = Color.Lerp(Color.Red,Color.BlueViolet,chaseFactor);
            Color trailBaseWhiteColor = Color.White.AdditiveColor() * alpha * 0.4f;
            Color trailBaseColor = color.AdditiveColor() * alpha * 0.4f;
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                if (Projectile.oldPos[i] == Vector2.Zero) continue;
                float factor = 1f - (float)i / Projectile.oldPos.Length;
                float rot = i == 0 ? Projectile.rotation : Projectile.oldRot[i];
                Vector2 drawpos = Projectile.oldPos[i] + Projectile.Size / 2f - Main.screenPosition;
                Color trailWhiteColor = trailBaseWhiteColor * factor * 0.9f;
                Color trailColor = trailBaseColor * factor * 0.9f;
                if (CollisionLeft < 1)
                {
                    trailWhiteColor *= 0.4f;
                    trailColor *= 0.4f;
                }
                float scale = Projectile.scale * factor;
                Main.spriteBatch.Draw(whitePart, drawpos, null, trailWhiteColor, rot, origin, scale, 0, 0);
                Main.spriteBatch.Draw(colorPart, drawpos, null, trailColor, rot, origin, scale, 0, 0);

            }
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, new Color(100,100,100), Projectile.rotation, origin, Projectile.scale, 0);
            Main.EntitySpriteDraw(whitePart, Projectile.Center - Main.screenPosition, null, whiteColor.AdditiveColor(), Projectile.rotation, origin, Projectile.scale, 0);

            Main.EntitySpriteDraw(colorPart, Projectile.Center - Main.screenPosition, null, color.AdditiveColor() * alpha * 1f, Projectile.rotation, origin, Projectile.scale, 0);

            return false;
        }
    }

    public class Needle : ModProjectile
    {
        public override string Texture => AssetDirectory.Projectiles + Name;

        public override void SetStaticDefaults()
        {
            Projectile.SetTrail(2, 8);
        }
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 36;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 10;
            Projectile.timeLeft = 40;
            Projectile.scale = 0.7f;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
        }

        public override void AI()
        {
            float factor = Projectile.TimeleftFactor();

            Projectile.Opacity = factor;

            Projectile.velocity *= 0.91f;
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Vector2 origin = texture.Size() / 2;

            float alpha = Projectile.Opacity;
            Color color = Color.Red.AdditiveColor() * alpha * 0.6f;
            Color trailBaseColor = new Color(192, 192, 192).AdditiveColor() * alpha * 0.7f;
            Vector2 scale = new Vector2(0.7f, 1f) * Projectile.scale;
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                if (Projectile.oldPos[i] == Vector2.Zero) continue;
                float factor = 1f - (float)i / Projectile.oldPos.Length;
                float rot = i == 0 ? Projectile.rotation : Projectile.oldRot[i];
                Vector2 drawpos = Projectile.oldPos[i] + Projectile.Size / 2f - Main.screenPosition;
                Color trailColor = trailBaseColor * factor * 0.6f;

                Main.spriteBatch.Draw(texture, drawpos, null, trailColor, rot, origin, scale, 0, 0);

            }

            RenderHelper.DrawBloom(10, 2, texture, Projectile.Center - Main.screenPosition, null, color * 0.4f, Projectile.rotation, origin, scale);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, new Color(210, 210, 210) * alpha, Projectile.rotation, origin, scale, 0);

            return false;
        }

    }
}

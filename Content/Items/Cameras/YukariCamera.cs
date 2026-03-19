using AyaMod.Common.Easer;
using AyaMod.Content.Particles;
using AyaMod.Core;
using AyaMod.Core.Prefabs;
using AyaMod.Core.Systems;
using AyaMod.Core.Systems.Trails;
using AyaMod.Helpers;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;
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
        public ref float Rotation => ref Projectile.localAI[0];
        public ref float ItemTimer => ref Projectile.localAI[1];
        public override void OnSpawn(IEntitySource source)
        {
            Vector2 pos = player.Center/*AyaUtils.RandAngle.ToRotationVector2() * 400 + player.GetModPlayer<CameraPlayer>().MouseWorld*/;
            Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), pos, Vector2.Zero, ProjectileType<YinYangBall>(), Projectile.damage, 12, Projectile.owner,Projectile.whoAmI);

            for(int i = 0; i < 2; i++)
            {
                Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ProjectileType<ReimuCircle>(), 0, 0f, player.whoAmI, Projectile.whoAmI, i);
            }
        }
        public override void HoverProjectile(Projectile projectile)
        {
            if (projectile.type != ProjectileType<YinYangBall>()) return;
            foreach(var proj in Main.ActiveProjectiles)
            {
                if(proj.type != ProjectileType<ReimuCircle>()) continue;
                proj.localAI[1] = projectile.Center.X;
                proj.localAI[2] = projectile.Center.Y;
            }
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


                foreach(var proj in Main.ActiveProjectiles)
                {
                    if (proj.type != ProjectileType<ReimuCircle>()) continue;
                    Color ballColor = proj.ai[1] > 0 ? new Color(255, 140, 140) : new Color(210,210,210);

                    int segments = (int)(proj.Distance(projectile.Center) / 18);
                    float rot = proj.AngleToSafe(projectile.Center);
                    for (int i = 0; i < segments; i++)
                    {
                        int echoform = 1;if (Main.rand.NextBool(3)) echoform++;
                        for (int j = 0; j < echoform; j++)
                        {
                            Vector2 pos2 = Vector2.Lerp(proj.Center, projectile.Center, i / (float)segments);
                            var star = StarParticle.Spawn(projectile.GetSource_FromAI(), pos2, Vector2.Zero, ballColor.AdditiveColor() * proj.Opacity,
                                1.5f, 0.25f, 0.75f, 0.92f + 0.03f * echoform, 1f, rot, 1f);
                            star.SetScaleMult(0.96f - 0.03f * echoform);
                        }
                    }

                }

                {
                    int dustamount = 48;
                    float rot = AyaUtils.RandAngle;
                    for (int i = 0; i < dustamount; i++)
                    {
                        int dustType = Main.rand.NextBool() ? DustID.RedTorch : DustID.WhiteTorch;
                        float dirr = MathHelper.TwoPi / dustamount * i;
                        Vector2 vec = dirr.ToRotationVector2();
                        float scale = Utils.Remap(MathF.Abs(vec.X), 0f, 1f, 1f, 0.5f);
                        vec = vec.RotatedBy(rot);
                        Dust d = Dust.NewDustPerfect(projectile.Center + vec * 16f, dustType, vec * 2.5f, Scale: scale * 5f);
                        d.noGravity = true;
                    }
                }
            }

        }
        public override void PostSnap()
        {
            if (Projectile.localAI[2] < 1)
            {
                int damage = (int)(Projectile.damage * 0.2f);
                foreach (var p in Main.ActiveProjectiles)
                {
                    if (p.type != ProjectileType<ReimuCircle>()) continue;

                    float baserot = AyaUtils.RandAngle;
                    int frame = Main.rand.Next(2);
                    for (int i = 0; i < 2; i++)
                    {
                        float rot = baserot + MathHelper.Pi * i;
                        Vector2 dir = rot.ToRotationVector2();
                        Vector2 vel = dir * 20;
                        var proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), p.Center, vel, ProjectileType<ReimuTailsman>(), damage, Projectile.knockBack / 4f, Projectile.owner);
                        proj.frame = frame;
                    }


                    {
                        int dustamount = 24;
                        float rot = AyaUtils.RandAngle;
                        for (int i = 0; i < dustamount; i++)
                        {
                            int dustType = Main.rand.NextBool() ? DustID.RedTorch : DustID.PurpleTorch;
                            float dirr = MathHelper.TwoPi / dustamount * i;
                            Vector2 vec = dirr.ToRotationVector2();
                            vec = vec.RotatedBy(rot);
                            Dust d = Dust.NewDustPerfect(p.Center + vec * 16f, dustType, vec * 4.5f, Scale: 5f);
                            d.noGravity = true;
                        }
                    }
                }
            }
            else Projectile.localAI[2] = 0;
        }
        public override void PostAI()
        {
            if (player.controlUseItem) ItemTimer++;
            else ItemTimer--;
            ItemTimer = MathHelper.Clamp(ItemTimer, 0, 30);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D bloom = Request<Texture2D>(AssetDirectory.Extras + "Ball6",ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;

            float itemFactor = EaseManager.Evaluate(Ease.InOutSine, ItemTimer, 30);
            float scaleFactor = 1f + 0.5f * itemFactor;
            float alphaFactor = 0.4f + 0.3f * itemFactor;
            if (!CanHit) alphaFactor *= 0.5f;

            float bloomScale = Projectile.scale * 584f / 512f * scaleFactor;
            Main.EntitySpriteDraw(bloom, Projectile.Center - Main.screenPosition, null, new Color(255, 104, 104).AdditiveColor() * (0.25f) * (alphaFactor - 0.25f * itemFactor),
                Rotation, bloom.Size() / 2, bloomScale, 0, 0);

            Texture2D yyy = Request<Texture2D>(AssetDirectory.Extras + "YinYangYu_Alpha", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;

            float radius = 210f * scaleFactor;

            RenderHelper.DrawRing(180, Projectile.Center, radius, Color.White * Projectile.Opacity * 0.4f * alphaFactor, Rotation, new Vector2(0.2f, 0.8f) * 0.6f);

            RenderHelper.DrawRing(180, Projectile.Center, radius * 0.9f, Color.White * Projectile.Opacity * 0.2f * alphaFactor, Rotation, new Vector2(0.2f, 0.8f) * 0.6f);

            Main.EntitySpriteDraw(yyy,Projectile.Center - Main.screenPosition, null, Color.White * 0.25f * Projectile.Opacity * alphaFactor, 
                Rotation + MathHelper.PiOver2 + MathHelper.Pi, yyy.Size() / 2, Projectile.scale * 420f / 960f * scaleFactor, 0, 0);

            return base.PreDraw(ref lightColor);
        }
    }
    public class ReimuCircle : ModProjectile
    {
        public override string Texture => AssetDirectory.EmptyTexturePass;
        public override void Load()
        {
            CameraSystem.OnPreUpdateProjectiles += ClearFlags;
            trail = new MultedTrail();
        }

        public static void ClearFlags()
        {
            foreach(var p in Main.ActiveProjectiles)
            {
                if (p.type != ProjectileType<ReimuCircle>()) continue;
                p.localAI[1] = -1;
                p.localAI[2] = -1;
            }
        }

        public ref float Owner => ref Projectile.ai[0];
        public ref float Offset => ref Projectile.ai[1];
        public ref float ItemTimer => ref Projectile.ai[2];
        public ref float ItemTimeFactor => ref Projectile.localAI[0];
        public ref float HighlightX => ref Projectile.localAI[1];
        public ref float HighlightY => ref Projectile.localAI[2];
        public static MultedTrail trail;
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 1;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }
        public override bool? CanDamage() => false;
        public override void OnSpawn(IEntitySource source)
        {
            Projectile.Opacity = 0f;
        }
        public override void AI()
        {
            Projectile.timeLeft++;

            float clickingFactor = EaseManager.Evaluate(Ease.InOutSine, ItemTimer, 30)/*ItemTimer / 30f*/;

            float radius = 100f + 50f * clickingFactor;
            float controlFactor = ItemTimer / 30f;

            Projectile camera = Main.projectile[(int)Owner];
            if (camera.TypeAlive(ProjectileType<YukariCameraProj>()))
            {
                if (Projectile.Opacity < 1f)
                    Projectile.Opacity += 0.015f;
                var cameraproj = camera.ModProjectile as YukariCameraProj;

                var player = cameraproj.player;
                if (!player.AliveCheck(Projectile.Center, 2000))
                {
                    Projectile.Kill(); return;
                }

                ItemTimer = cameraproj.ItemTimer;
                ItemTimeFactor = Utils.Remap(player.itemTime, 1, player.itemTimeMax, 1f, 0f);
                if (player.ItemTimeIsZero) ItemTimeFactor = 0;
                Vector2 targetpos = camera.Center + (Offset * MathHelper.Pi + cameraproj.Rotation).ToRotationVector2() * radius;
                Projectile.Center = Vector2.Lerp(Projectile.Center, targetpos, 0.9f);

                float revolutionSpeed = 0.015f + controlFactor * 0.005f;
                cameraproj.Rotation += revolutionSpeed / 2f;
            }
            else Projectile.Kill();

            Projectile.rotation += 0.02f + controlFactor * 0.01f;
        }

        public override bool PreDraw(ref Color lightColor)
        {

            Texture2D texture = TextureAssets.Extra[98].Value;
            Vector2 origin = texture.Size() / 2;
            float clickingFactor = EaseManager.Evaluate(Ease.InOutSine, ItemTimer, 30);

            Color colora = new Color(72, 72, 72);
            Color colorb = new Color(216, 216, 216);
            Color colorc = new Color(249,62,52) * 0.8f;
            Color colord = new Color(255, 93, 85);
            Color baseColor = Color.Lerp(colora, colorb, clickingFactor);
            Color baseColor2 = Color.Lerp(colorc, colord, clickingFactor);
            Color color = (Offset > 0 ? baseColor2 : baseColor).AdditiveColor() * Projectile.Opacity;
            Color colorReversed = (Offset > 0 ? baseColor : baseColor2).AdditiveColor() * Projectile.Opacity;
            float ringRadius = 38f + 12f * clickingFactor;

            RenderHelper.DrawRing(72, Projectile.Center, ringRadius, color, Projectile.rotation, new Vector2(0.25f, 0.8f) * 0.6f);
            RenderHelper.DrawRing(72, Projectile.Center, ringRadius * (0.8f - clickingFactor * 0.1f), color * 0.6f, Projectile.rotation, new Vector2(0.25f, 0.8f) * 0.6f);
            
            Vector2 offset = Projectile.Center - Main.screenPosition;
            Vector2 scale1 = new(8 / 64f, 24 / 64f);

            for (int j = 0; j < 2; j++)
            {
                int jr = j == 0 ? 1 : 0;
                float radius = ringRadius * (0.6f + j * 0.4f - 0.3f * clickingFactor + jr * clickingFactor * 0.7f - j * clickingFactor * 0.3f);
                float rotmodifier = 1f;
                //rotmodifier -= j * clickingFactor * 0.3f;
                int extra = 2;
                for (int l = 0; l < extra; l++)
                {
                    float extraRot = Projectile.rotation * rotmodifier + l * MathHelper.PiOver4;
                    for (int i = 0; i < 4; i++)
                    {
                        float dir1 = MathHelper.TwoPi / 4 * i + extraRot;
                        float dir2 = MathHelper.TwoPi / 4 * (i + 1) + extraRot;
                        Vector2 vec1 = dir1.ToRotationVector2() * radius;
                        Vector2 vec2 = dir2.ToRotationVector2() * radius;
                        Vector2 middle = Vector2.Lerp(vec1, vec2, 0.5f) * (1.33f - jr * 0.33f + clickingFactor * 0.25f);

                        var dist = (int)(vec1.Distance(middle) / 4f);
                        var tomiddle = vec1.AngleTo(middle);
                        var tovec2 = middle.AngleTo(vec2);
                        for (int k = 1; k < dist; k++)
                        {
                            float factor = k / (float)dist;
                            Vector2 pos1 = Vector2.Lerp(vec1, middle, factor);
                            Main.spriteBatch.Draw(texture, pos1 + offset, null, color, tomiddle + MathHelper.PiOver2, origin, scale1, 0, 0);

                            Vector2 pos2 = Vector2.Lerp(middle, vec2, factor);
                            Main.spriteBatch.Draw(texture, pos2 + offset, null, color, tovec2 + MathHelper.PiOver2, origin, scale1, 0, 0);
                        }

                    }
                }
            }

            for (int i = 0; i < 5; i++)
            {
                float rotmodifier = 0.7f;
                float dir1 = MathHelper.TwoPi / 5 * i + -Projectile.rotation * rotmodifier;
                float radius = ringRadius * (1 - clickingFactor * 0.3f);
                Vector2 vec1 = dir1.ToRotationVector2() * radius;
                Vector2 tonext = (dir1 + MathHelper.ToRadians(126)).ToRotationVector2();

                var dist = (int)(radius * (0.65f) * MathF.Sin(MathHelper.ToRadians(36)));
                for (int j = 1; j < dist - 3; j++)
                {
                    float factor = j / (float)dist;
                    Vector2 pos = vec1 + tonext * factor * dist * 4f;
                    Main.spriteBatch.Draw(texture, pos + offset, null, colorReversed, tonext.ToRotation() + MathHelper.PiOver2, origin, scale1, 0, 0);
                }

            }

            Color ballColor = (Offset > 0 ? new Color(255, 140, 140) : colorb).AdditiveColor() * Projectile.Opacity;
            {
                Texture2D ball1 = Request<Texture2D>(AssetDirectory.Extras + "Ball1", AssetRequestMode.ImmediateLoad).Value;
                Vector2 pos = Projectile.Center - Main.screenPosition;
                float itemFactor = ItemTimeFactor;
                for (int j = 0; j < 5; j++)
                {
                    Main.spriteBatch.Draw(ball1, pos, null, ballColor * 0.6f * itemFactor, Main.GameUpdateCount * 0.1f, ball1.Size() / 2, Projectile.scale * (0.8f - j * 0.15f) * 0.6f, 0, 0);

                }

            }

            if(HighlightX > 0 && HighlightY > 0)
            {
                Vector2 targetPos = new(HighlightX, HighlightY);
                float distance = Projectile.Distance(targetPos);
                Vector2[] poses = new Vector2[(int)(distance / 8)];
                for(int i = 0; i < poses.Length; i++)
                {
                    float factor = (float)i / poses.Length;
                    poses[i] = Vector2.Lerp(Projectile.Center,targetPos, factor);
                }
                trail.PrepareStrip(poses, 2, factor => Color.Lerp(Color.Black, ballColor, ItemTimeFactor).AdditiveColor(),
                    factor => 8f, -Main.screenPosition, factor => Utils.Remap(factor, 0, 0.1f, 0f, 1f) * Utils.Remap(factor, 0.9f, 1f, 1f, 0f));
                Main.graphics.GraphicsDevice.Textures[0] = TextureAssets.Extra[197].Value;
                trail.DrawTrail();

            }
            return false;
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
        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            width = height = 40;
            return base.TileCollideStyle(ref width, ref height, ref fallThrough, ref hitboxCenterFrac);
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
    public class ReimuTailsman : ModProjectile
    {
        public override string Texture => AssetDirectory.Projectiles + Name;
        public static MultedTrail strip = new();
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 2;
            Projectile.SetTrail(4, 25);
        }
        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;

            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 240;
            Projectile.scale = 0.75f;
            Projectile.SetImmune(20);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.ai[0]++;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.ai[0]++;
            Projectile.velocity = oldVelocity;
            return false;
        }
        public override void AI()
        {
            float chaseFactor = Utils.Remap(Projectile.timeLeft, 180, 240, 0.06f, 0.01f);
            Projectile.Chase(2000, 22, chaseFactor);

            if (Projectile.ai[0] > 0)
            {
                Projectile.velocity *= 0.92f;
                Projectile.Opacity -= 0.05f;
                if (Projectile.Opacity <= 0.05f) Projectile.Kill();
            }

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
        }

        public Color ColorFunction(float progress)
        {
            Color drawColor = Projectile.frame == 0 ? new(255,119,102) : new(227,87,241);
            return Color.Lerp(drawColor * .5f, Color.Black, progress).AdditiveColor() * Projectile.Opacity;
        }
        public float WidthFunction(float progress)
        {
            return 10f * Projectile.scale;
        }
        public float AlphaFunction(float progress)
        {
            float fadeinFactor = Utils.Remap(progress, 0f, 0.05f, 0f, 1f);
            return EaseManager.Evaluate(Ease.Linear, 1f - progress, 1f) * Projectile.Opacity * fadeinFactor;
        }
        public override bool PreDraw(ref Color lightColor)
        {

            #region 拖尾
            strip.PrepareStrip(Projectile.oldPos, 2, ColorFunction, WidthFunction,
                Projectile.Size / 2 - Main.screenPosition, AlphaFunction);
            Main.graphics.GraphicsDevice.Textures[0] = TextureAssets.MagicPixel.Value;
            strip.DrawTrail();

            #endregion


            Texture2D texture = TextureAssets.Projectile[Type].Value;

            int frameWidth = texture.Width / Main.projFrames[Type];
            int x = frameWidth * Projectile.frame;

            Rectangle rect = new(x, 0, 24, 30);
            Color color = Color.White * Projectile.Opacity;

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, rect,
                color, Projectile.rotation, Projectile.Size / 2, Projectile.scale, 0);

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

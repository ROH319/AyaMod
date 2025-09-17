using AyaMod.Core.Prefabs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Enums;
using Terraria.ID;
using Terraria;
using AyaMod.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Audio;
using AyaMod.Core;

namespace AyaMod.Content.Items.Cameras
{
    public class DukeCamera : BaseCamera
    {
        public override void SetOtherDefaults()
        {

            Item.width = 52;
            Item.height = 48;

            Item.damage = 340;

            Item.useTime = Item.useAnimation = 70;
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.shoot = ModContent.ProjectileType<DukeCameraProj>();
            Item.shootSpeed = 8;
            Item.knockBack = 8f;

            Item.SetShopValues(ItemRarityColor.Yellow8, Item.sellPrice(0, 5, 0, 0));
            SetCameraStats(0.07f, 182, 1.5f);
            SetCaptureStats(100, 5);
        }
    }

    public class DukeCameraProj : BaseCameraProj
    {
        public override Color outerFrameColor => new Color(53, 220, 255);
        public override Color innerFrameColor => new Color(211, 158, 255);
        public override Color focusCenterColor => base.focusCenterColor;
        public override Color flashColor => new Color(243, 209, 255).AdditiveColor() * 0.5f;

        public override void OnSnapInSight()
        {
            if (!Projectile.MyClient()) return;
            int damage = (int)(Projectile.damage * 0.2f);
            Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ProjectileType<DukeWave>(), damage, Projectile.knockBack, Projectile.owner);
        }

        public override void PostAI()
        {
            if(!player.ItemTimeIsZero && player.itemTime % 5 == 0)
            {
                var vel = Main.rand.NextVector2Unit() * Main.rand.NextFloat(4, 6);
                int damage = (int)(Projectile.damage * 0.17f);
                Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, vel, ProjectileType<FantasticBubble1>(), damage, Projectile.knockBack * 2f, Projectile.owner);
            }
        }
    }

    public class FantasticBubble1 : ModProjectile
    {
        public override string Texture => AssetDirectory.Projectiles + Name;
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 18;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.SetImmune(10);
            Projectile.timeLeft = 2 * 60;
        }
        public override void OnSpawn(IEntitySource source)
        {
            Projectile.Opacity = 0;
        }
        public override bool? CanDamage()
        {
            return Projectile.Opacity > 0.8f;
        }
        public override void AI()
        {
            //弹幕生成的前几帧不追踪
            if (Projectile.Opacity > 0.8f)
            {
                var chasing = Projectile.Chase(350, 8, 0.01f);
                if (!chasing) Projectile.velocity *= 0.93f;
                else Projectile.velocity *= 0.97f;
                if (Projectile.velocity.Length() < 0.1f) Projectile.timeLeft -= 1;

            }
            Projectile.Opacity += 0.06f;
            if(Projectile.Opacity > 1f)Projectile.Opacity = 1f;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item54, Projectile.Center);

            for (int num261 = 0; num261 < 20; num261++)
            {
                int num262 = 10;
                int num263 = Dust.NewDust(Projectile.Center - Vector2.One * num262, num262 * 2, num262 * 2, 212);
                Dust dust40 = Main.dust[num263];
                Vector2 vector34 = Vector2.Normalize(dust40.position - Projectile.Center);
                dust40.position = Projectile.Center + vector34 * num262 * Projectile.scale;
                if (num261 < 30)
                    dust40.velocity = vector34 * dust40.velocity.Length();
                else
                    dust40.velocity = vector34 * Main.rand.Next(45, 91) / 10f;

                dust40.color = Main.hslToRgb((float)(0.7000000059604645 + Main.rand.NextDouble() * 0.20000000298023224), 0.9f, 0.7f);
                dust40.color = Color.Lerp(dust40.color, Color.White, 0.5f);
                dust40.noGravity = true;
                dust40.scale = 0.7f;
            }
        }
    }

    public class FantasticBubble2 : ModProjectile
    {
        public override string Texture => AssetDirectory.Projectiles + Name;
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.SetImmune(10);
            Projectile.timeLeft = (int)(180 + MathF.Cos(Projectile.whoAmI * 0.5f) * 20f);
        }
        public override void OnSpawn(IEntitySource source)
        {
            Projectile.Opacity = 0;

        }
        public override bool? CanDamage()
        {
            return Projectile.Opacity > 0.5f;
        }
        public override void AI()
        {
            // 弹幕生成的前几帧不追踪
            if (Projectile.timeLeft <= 165)
            {
                var chasing = Projectile.Chase(800, 22 + MathF.Sin(Projectile.whoAmI * 0.5f) * 5f, 0.05f);
                if (!chasing)
                {
                    // 不追踪时向上飘走
                    Projectile.velocity.X *= 0.97f;
                    Projectile.velocity.Y = MathHelper.Lerp(Projectile.velocity.Y, -(2.4f + MathF.Sin(Projectile.whoAmI * 0.5f) * 0.6f), 0.05f);
                }
                //else Projectile.velocity *= 0.97f;
            }
            else Projectile.velocity *= 0.96f;
            if (Projectile.velocity.Length() < 0.1f) Projectile.timeLeft -= 1;

            Projectile.Opacity += 0.1f;
            if (Projectile.Opacity > 1f) Projectile.Opacity = 1f;
        }
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item54, Projectile.Center);

            for (int num261 = 0; num261 < 25; num261++)
            {
                int num262 = 10;
                int num263 = Dust.NewDust(Projectile.Center - Vector2.One * num262, num262 * 2, num262 * 2, DustID.BubbleBurst_White);
                Dust dust40 = Main.dust[num263];
                Vector2 vector34 = Vector2.Normalize(dust40.position - Projectile.Center);
                dust40.position = Projectile.Center + vector34 * num262 * Projectile.scale;
                if (num261 < 30)
                    dust40.velocity = vector34 * dust40.velocity.Length() * 1.3f;
                else
                    dust40.velocity = vector34 * Main.rand.Next(45, 91) / 10f;

                dust40.color = Main.hslToRgb((float)(0.7000000059604645 + Main.rand.NextDouble() * 0.20000000298023224), 0.9f, 0.7f);
                dust40.color = Color.Lerp(dust40.color, Color.White, 0.5f);
                dust40.noGravity = true;
                dust40.scale = 1f;
            }
        }
    }

    public class DukeWave : ModProjectile
    {
        public override string Texture => AssetDirectory.Extras + "Ball_1";
        public int TrailCount = 10;

        public ref float CurrentRadius => ref Projectile.ai[0];

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 48;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
            Projectile.timeLeft = 60;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.oldRot = new float[TrailCount];
            for (int i = 0; i < TrailCount; i++)
            {
                Projectile.oldRot[i] = 25;
            }
            CurrentRadius = 25;

            for (int i = 0; i < 2; i++)
            {
                SoundEngine.PlaySound(SoundID.Splash with
                {
                    MaxInstances = 30,
                    Volume = 1f
                }, Projectile.Center);
            }
        }

        public override bool? CanHitNPC(NPC target)
        {
            return target.noTileCollide || Collision.CanHitLine(Projectile.Center, 1, 1, target.position, target.width, target.height);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return Helper.CheckRingCollision(targetHitbox, Projectile.Center, CurrentRadius - 15, CurrentRadius + 10);
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            float factor = Projectile.TimeleftFactor();
            modifiers.FinalDamage *= Utils.Remap(factor, 0, 1, 0.5f, 1f);
        }

        public override void AI()
        {
            float factor = Projectile.TimeleftFactor();
            float vel = 1f;

            vel = Utils.Remap(factor, 1f, 0f, 10f, 0f);
            Projectile.Opacity = factor;
            Projectile.oldRot[0] = CurrentRadius;
            for (int i = TrailCount - 1; i > 0; i--)
            {
                Projectile.oldRot[i] = Projectile.oldRot[0] - i * 8;
            }

            CurrentRadius += vel * 0.8f;

            foreach (Projectile projectile in Main.ActiveProjectiles)
            {
                if (projectile.type != ModContent.ProjectileType<FantasticBubble1>()) continue;
                if ((bool)!Colliding(Projectile.Hitbox, projectile.GetHitbox())) continue;

                Vector2 push = Projectile.DirectionToSafe(projectile.Center) * vel;
                Projectile newbubble = Projectile.NewProjectileDirect(projectile.GetSource_FromAI(),projectile.Center,projectile.velocity + push,ModContent.ProjectileType<FantasticBubble2>(),projectile.damage,projectile.knockBack,projectile.owner);

                projectile.Kill();

            }

            int dustcount = 32;
            for (int i = 0; i < dustcount; i++)
            {
                Vector2 pos = Projectile.Center + (Main.rand.NextFloat(MathHelper.TwoPi) + MathHelper.Pi).ToRotationVector2() * CurrentRadius;
                Dust d = Dust.NewDustPerfect(pos, DustID.Water, Projectile.Center.DirectionToSafe(pos) * vel * 0.2f);
                d.noGravity = true;
            }

            int checkCount = (int)Utils.Remap(CurrentRadius, 25, 500, 4, 46);
            for (int i = 0; i < checkCount - 1; i++)
            {
                Vector2 p1 = Projectile.Center + (MathHelper.TwoPi / (float)checkCount * i).ToRotationVector2() * CurrentRadius;
                Vector2 p2 = Projectile.Center + (MathHelper.TwoPi / (float)checkCount * (i + 1)).ToRotationVector2() * CurrentRadius;
                Utils.PlotTileLine(p1, p2, 3, new Utils.TileActionAttempt(DelegateMethods.CutTiles));
            }
            {
                Vector2 p1 = Projectile.Center + (MathHelper.TwoPi / (float)checkCount * (checkCount - 1)).ToRotationVector2() * CurrentRadius;
                Vector2 p2 = Projectile.Center + Vector2.UnitX * CurrentRadius;
                Utils.PlotTileLine(p1, p2, 3, new Utils.TileActionAttempt(DelegateMethods.CutTiles));

            }
        }

        public override bool PreDraw(ref Color lightColor)
        {

            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Texture2D star = AssetDirectory.StarTexture;

            float timeleftFactor = Projectile.TimeleftFactor();

            int drawcount = (int)Utils.Remap(CurrentRadius, 25, 500, 25, 500);
            for (int j = 0; j < drawcount; j++)
            {
                float rotFactor = MathHelper.TwoPi / drawcount * j;

                for (int i = 0; i < TrailCount; i++)
                {
                    Vector2 pos = Projectile.Center + rotFactor.ToRotationVector2() * Projectile.oldRot[i];
                    float trailFactor = i / (float)TrailCount;

                    float alphaFactor = Utils.Remap(trailFactor, 0, 1f, 1f, 0);
                    alphaFactor *= Utils.Remap(timeleftFactor, 0, 1, 1f, 0.6f);
                    //new Color(96, 232, 255)
                    Color baseColor = new Color(196, 160, 255) * (Projectile.GetAlpha(lightColor).A / 255f); ;
                    if (!Main.dayTime) baseColor = baseColor.AdditiveColor() * 1f;
                    Color color = baseColor * alphaFactor * 0.09f * Projectile.Opacity;
                    if (i == 0) color *= 2f;
                    Main.spriteBatch.Draw(texture, pos - Main.screenPosition, null, color, Projectile.rotation, texture.Size() / 2, Projectile.scale * 0.6f, 0, 0);
                }

                {
                    Color bloomcolor = Color.White.AdditiveColor() * 0.12f * Projectile.Opacity;
                    Vector2 pos = Projectile.Center + rotFactor.ToRotationVector2() * MathHelper.Lerp(Projectile.oldRot[0], Projectile.oldRot[1], 0f) - Main.screenPosition;
                    Main.spriteBatch.Draw(star, pos, null, bloomcolor, rotFactor, star.Size() / 2, new Vector2(0.5f, 1f), 0, 0);
                }
            }

            return false;
        }
    }
}

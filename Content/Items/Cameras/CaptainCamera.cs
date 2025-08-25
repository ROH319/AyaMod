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
using Terraria.DataStructures;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using AyaMod.Common.Easer;
using ReLogic.Content;
using Terraria.Audio;

namespace AyaMod.Content.Items.Cameras
{
    public class CaptainCamera : BaseCamera
    {

        public override void SetOtherDefaults()
        {
            Item.width = 52;
            Item.height = 48;

            Item.damage = 70;

            Item.useTime = Item.useAnimation = 45;
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.shoot = ModContent.ProjectileType<CaptainCameraProj>();
            Item.shootSpeed = 8;
            Item.knockBack = 8f;

            Item.SetShopValues(ItemRarityColor.Orange3, Item.sellPrice(0, 0, 78, 0));
            SetCameraStats(0.05f, 132, 1.4f,0.5f);
            SetCaptureStats(100, 5);
        }

        public override void AddRecipes()
        {
        }
    }

    public class CaptainCameraProj : BaseCameraProj
    {
        public override Color outerFrameColor => new Color(55, 86, 178);
        public override Color innerFrameColor => new Color(115, 209, 234) * 0.7f;
        public override Color focusCenterColor => base.focusCenterColor;
        public override Color flashColor => new Color(187, 206, 245).AdditiveColor() * 0.5f;

        public override void OnSnapInSight()
        {
            int dustcount = 64;
            for(int i = 0; i < dustcount; i++)
            {
                Vector2 dir = Main.rand.NextVector2Unit();
                float speed = Main.rand.NextFloat(4, 6);
                Vector2 pos = Projectile.Center + dir * Main.rand.NextFloat(60, 80);
                Dust d = Dust.NewDustPerfect(pos, DustID.Water, dir * speed,Scale:1.7f);
                d.noGravity = true;
            }

            if (!Projectile.MyClient()) return;
            if (++EffectCounter >= 4)
            {

                float speed = 10f;
                float rotdir = Main.rand.NextBool() ? -1 : 1;
                for (int i = 0; i < 8; i++)
                {
                    var waveCreater = i == 0 ? 1 : 0;

                    float rot = MathHelper.TwoPi / 8f * i;
                    Vector2 dir = rot.ToRotationVector2();
                    Vector2 vel = dir * speed;

                    float rotspeed = MathHelper.TwoPi / 90f;
                    Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, vel, ModContent.ProjectileType<CaptainAnchor>(), (int)(Projectile.damage * 1.3f), Projectile.knockBack * 2f, Projectile.owner, rotspeed, rotdir, waveCreater);
                }
                EffectCounter = 0;
            }


        }

        public override void PostAI()
        {
            //if (ComputedVelocity.Length() > 8)
            //{
            //    int dustcount = 3;
            //    for (int i = 0; i < dustcount; i++)
            //    {
            //        Vector2 pos = Projectile.Center + Main.rand.NextVector2Unit() * Main.rand.NextFloat(5, 150);
            //        Dust d = Dust.NewDustPerfect(pos, DustID.Water, ComputedVelocity * 0.6f, Scale: Main.rand.NextFloat(1, 1.5f));
            //        d.noGravity = true;
            //    }
            //}
            base.PostAI();
        }
    }

    public class CaptainAnchor : ModProjectile
    {
        public override string Texture => AssetDirectory.Projectiles + Name;

        public ref float rotSpeed => ref Projectile.ai[0];
        public ref float rotDir => ref Projectile.ai[1];
        public ref float waveCreater => ref Projectile.ai[2];

        public override void SetStaticDefaults()
        {
            Projectile.SetTrail(4, 14);
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 42;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 90;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 10;
            Projectile.scale = 1.5f;
        }

        public override void OnSpawn(IEntitySource source)
        {
            SoundEngine.PlaySound(SoundID.Splash with
            {
                MaxInstances = 30,
                Volume = 1f
            }, Projectile.Center);
        }

        public override void AI()
        {
            for(int i = 0;i<3;i++)
            {
                Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, 253, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f,Scale:1f);
                d.velocity = d.velocity.RotatedByRandom(0.3f);
                d.noGravity = true;
            }

            Projectile.velocity = Projectile.velocity.RotatedBy(rotSpeed * rotDir);

            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override void OnKill(int timeLeft)
        {
            if (waveCreater == 1)
            {
                Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<CaptainWave>(), (int)(Projectile.damage * 1.3f), Projectile.knockBack, Projectile.owner);
            }

            int dustcount = 8;

            for(int i = 0; i < dustcount; i++)
            {
                Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, 111, Projectile.velocity.X * 0.85f, Projectile.velocity.Y * 0.85f,Scale:1.6f);
                d.noGravity = true;
            }

            SoundEngine.PlaySound(SoundID.Shimmer1 with
            {
                MaxInstances = 30,
                Volume = 1f
            }, Projectile.Center);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;

            Texture2D star = TextureAssets.Extra[98].Value;

            Texture2D ball4 = ModContent.Request<Texture2D>(AssetDirectory.Extras + "Ball4", AssetRequestMode.ImmediateLoad).Value;
            Texture2D ball3 = ModContent.Request<Texture2D>(AssetDirectory.Extras + "Ball3", AssetRequestMode.ImmediateLoad).Value;

            Vector2 curdir = Projectile.rotation.ToRotationVector2();

            int length = Projectile.oldPos.Length;
            for(int i = 1; i < length - 1; i++)
            {
                float totalFactor = (float)i / length;
                var dir = Projectile.oldPos[i + 1] - Projectile.oldPos[i];
                
                if(dir != Vector2.Zero)
                {
                    float oldrot = Projectile.oldRot[i];
                    float alphaFactor = Utils.Remap(totalFactor, 0, 1f, 0.8f, 0);
                    alphaFactor = MathHelper.Lerp(alphaFactor, 0, EaseManager.Evaluate(Ease.OutSine, totalFactor, 1f));
                    Color baseColor = Color.Lerp(Color.LightSkyBlue, new Color(0,0,235), EaseManager.Evaluate(Ease.OutCubic,totalFactor,1f));
                    if (!Main.dayTime) baseColor = baseColor.AdditiveColor() * 0.7f;
                    Color color = baseColor * 1.3f * alphaFactor;

                    for(int j = -1; j < 2; j += 2)
                    {
                        Vector2 pos = Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition + new Vector2(0, 18f * j).RotatedBy(oldrot) * Projectile.scale;

                        Rectangle sourceRect = new Rectangle(18 - 18 * j, 0, 36, 72);
                        Vector2 origin = new Vector2(18 + 18 * j, 36);
                        Main.spriteBatch.Draw(star, pos, sourceRect, color, oldrot + MathHelper.PiOver2, origin, new Vector2(Projectile.scale * 1f, 1f), 0, 0);

                    }
                }
            }

            RenderHelper.DrawBloom(6, 3, texture, Projectile.Center - Main.screenPosition, null, Color.LightGreen.AdditiveColor(), Projectile.rotation, texture.Size() / 2, Projectile.scale);

            Main.spriteBatch.Draw(ball4, Projectile.Center + curdir * 6 - Main.screenPosition, null, Color.LightSkyBlue, Projectile.rotation, ball4.Size() / 2, Projectile.scale * 0.21f, 0, 0);
            Main.spriteBatch.Draw(ball4, Projectile.Center + curdir * 4 - Main.screenPosition, null, Color.White, Projectile.rotation, ball4.Size() / 2, Projectile.scale * 0.08f, 0, 0);
            //for (int i = 0; i < 4; i++)
            //{
            //    Main.spriteBatch.Draw(ball3, Projectile.Center - Main.screenPosition, null, Color.White.AdditiveColor(), Projectile.rotation, ball4.Size() / 2, Projectile.scale * 0.15f, 0, 0);
            //}
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, texture.Size() / 2, Projectile.scale, 0, 0);

            
            return false;
        }
    }

    public class CaptainWave : ModProjectile
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
        }

        public override bool? CanHitNPC(NPC target)
        {
            return target.noTileCollide || Collision.CanHitLine(Projectile.Center, 1, 1, target.position, target.width, target.height);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return Helper.CheckRingCollision(targetHitbox, Projectile.Center, CurrentRadius - 10, CurrentRadius + 10);
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

            vel = Utils.Remap(factor, 1f, 0f, 14f, 0f);
            Projectile.Opacity = factor;
            Projectile.oldRot[0] = CurrentRadius;
            for (int i = TrailCount - 1; i > 0; i--)
            {
                Projectile.oldRot[i] = Projectile.oldRot[0] - i * 10;
            }

            int dustcount = 48;
            for(int i = 0; i < dustcount; i++)
            {
                Vector2 pos = Projectile.Center + (Main.rand.NextFloat(MathHelper.TwoPi) + MathHelper.Pi).ToRotationVector2() * CurrentRadius;
                Dust d = Dust.NewDustPerfect(pos, DustID.Water, Projectile.Center.DirectionToSafe(pos) * vel * 0.2f);
                d.noGravity = true;
            }

            CurrentRadius += vel * 0.8f;
            //Main.NewText($"{CurrentRadius}");
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
                    Color baseColor = new Color(43, 0, 255) * (Projectile.GetAlpha(lightColor).A / 255f); ;
                    if(!Main.dayTime) baseColor = baseColor.AdditiveColor() * 1f;
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

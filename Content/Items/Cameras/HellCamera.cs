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
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using AyaMod.Content.Particles;
using AyaMod.Common.Easer;
using AyaMod.Core;

namespace AyaMod.Content.Items.Cameras
{
    public class HellCamera : BaseCamera
    {
        public override void SetOtherDefaults()
        {
            Item.width = 52;
            Item.height = 48;

            Item.damage = 36;

            Item.useTime = Item.useAnimation = 40;
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.shoot = ModContent.ProjectileType<HellCameraProj>();
            Item.shootSpeed = 8;
            Item.knockBack = 8f;

            Item.SetShopValues(ItemRarityColor.Orange3, Item.sellPrice(0, 0, 18, 0));
            SetCameraStats(0.04f, 108, 2f);
            SetCaptureStats(1000, 60);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.HellstoneBar, 15)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class HellCameraProj : BaseCameraProj
    {
        public override Color outerFrameColor => new Color(244, 42, 10);
        public override Color innerFrameColor => new Color(137, 216, 255) * 0.7f;
        public override Color focusCenterColor => base.focusCenterColor;
        public override Color flashColor => new Color(255, 251, 179).AdditiveColor() * 0.5f;

        public override void OnSnap()
        {
            if (!Projectile.MyClient()) return;

            int count = 3;

            int damage = (int)(Projectile.damage * 0.3f);
            for(int i = 0; i < count; i++)
            {
                Vector2 pos = Projectile.Center + AyaUtils.RandAngle.ToRotationVector2() * Main.rand.NextFloat(20, 80);
                Vector2 vel = AyaUtils.RandAngle.ToRotationVector2() * Main.rand.NextFloat(4, 7);
                Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), pos, vel, ProjectileType<HellSpiritShotHoming>(), damage, Projectile.knockBack, Projectile.owner, 1);
            }
            //int count = 1;
            //if (Main.rand.NextBool()) count++;
            //int damage = (int)(Projectile.damage * 0.4f);
            //for (int i = 0;i<count;i++)
            //{

            //    Vector2 pos = Projectile.Center + AyaUtils.RandAngle.ToRotationVector2() * Main.rand.NextFloat(20, 80);
            //    Vector2 vel = AyaUtils.RandAngle.ToRotationVector2() * Main.rand.NextFloat(4, 7);
            //    Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), pos, vel, ModContent.ProjectileType<HellSpirit>(), damage, Projectile.knockBack / 4, Projectile.owner);

            //}
            base.OnSnap();
        }
        public override void HoverProjectile(Projectile projectile)
        {
            if (projectile.type != ProjectileType<HellSpirit>()) return;
            projectile.ai[0]++;
        }
    }

    public class HellSpirit : ModProjectile
    {
        public override string Texture => AssetDirectory.Extras + "Ball1";

        public static Color SpiritBlue = new Color(60, 89, 255);
        public static Color SpiritPurple = new Color(186, 70, 255);

        public float HoverFactor => Projectile.ai[0] / 60f;
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 48;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
            Projectile.timeLeft = 90;
        }
        public override bool? CanDamage() => false;

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.velocity *= 0.6f;
            return false;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.Opacity = 0;
            Projectile.scale = 0.75f;
        }

        public override void AI()
        {
            if (Projectile.Opacity < 1f) Projectile.Opacity += 0.05f;
            //Main.NewText($"{HoverFactor} {Projectile.ai[0]}");
            Projectile.velocity *= 0.98f;
            if (Projectile.timeLeft % 4 == 0)
            {

                Color color = Color.Lerp(HellSpirit.SpiritBlue, HellSpirit.SpiritPurple, HoverFactor);
                Vector2 offset = AyaUtils.RandAngle.ToRotationVector2() * Main.rand.NextFloat(2, 12);
                var ball = SoulsParticle.Spawn(Projectile.GetSource_FromAI(), Projectile.Center + offset, Vector2.UnitY * -2.7f + Projectile.velocity * 0.8f, color.AdditiveColor());
                ball.SetAlphaFadeout(new Core.FloatModifier().SetAdditive(-0.015f));
                ball.SetScaleFadeout(new Core.FloatModifier().SetAdditive(-0.025f));
                ball.Scale = Projectile.scale;
            }
        }
        public override void OnKill(int timeLeft)
        {
            float angle = AyaUtils.RandAngle;
            NPC npc = Projectile.FindCloestNPC(600, true);
            if (npc != null)
            {
                angle = Projectile.AngleToSafe(npc.Center);
            }

            int damage = (int)(Projectile.damage * 0.8f);
            for (int i = 0; i < 3; i++)
            {
                int homing = -1;
                switch (i)
                {
                    case 0:if (npc != null) homing = 1;break;
                    case 1:if (HoverFactor > 0.6f) homing = 1;break;
                    case 2:if (HoverFactor > 0.9f) homing = 1;break;
                    default:break;
                }
                Vector2 dir = (MathHelper.TwoPi / 3 * i + angle).ToRotationVector2();
                Vector2 vel = dir * 4f;
                Projectile.NewProjectileDirect(Projectile.GetSource_Death(), Projectile.Center, vel, ModContent.ProjectileType<HellSpiritShot>(), damage, 0, Projectile.owner, homing);
            }

            int dustamount = 12;
            for(int i = 0; i < dustamount; i++)
            {
                float rot = AyaUtils.RandAngle;
                Color color = Color.Lerp(SpiritBlue, SpiritPurple, HoverFactor);

                Vector2 vel = rot.ToRotationVector2() * Main.rand.NextFloat(0.5f,2f);
                var soul = SoulsParticle.Spawn(Projectile.GetSource_FromAI(), Projectile.Center, vel, color);
                soul.SetAlphaFadeout(new Core.FloatModifier().SetAdditive(-0.05f));
                soul.SetScaleFadeout(new Core.FloatModifier().SetAdditive(-0.05f));
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;

            Color color = Color.Lerp(SpiritBlue, SpiritPurple, HoverFactor);
            float basebloomScale = 0.5f;
            float bloomScale = 0.6f;
            int repeat = 16;
            float maxAlpha = 0.25f;
            for(int i = 0; i < repeat; i++)
            {
                float factor = (float)i / repeat;
                Color bloomColor = Color.Lerp(Color.White, color, EaseManager.Evaluate(Ease.OutCubic, factor, 1f));
                float alpha = MathHelper.Lerp(maxAlpha, 0, EaseManager.Evaluate(Ease.OutCubic, factor, 1f)) * Projectile.Opacity;
                float scale = basebloomScale + MathHelper.Lerp(0, bloomScale, EaseManager.Evaluate(Ease.OutQuint, factor, 1f));

                Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, bloomColor.AdditiveColor() * alpha, Projectile.rotation, texture.Size() / 2, scale * Projectile.scale, 0, 0);

            }

            //Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, color.AdditiveColor() * Projectile.Opacity, Projectile.rotation, texture.Size() / 2, Projectile.scale * 0.6f, 0, 0);
            RenderHelper.DrawBloom(12, 4, texture, Projectile.Center - Main.screenPosition, null, color.AdditiveColor() * Projectile.Opacity, Projectile.rotation, texture.Size() / 2, Projectile.scale * 0.3f);


            return false;
        }
    }

    public class HellSpiritShot : ModProjectile
    {
        public override string Texture => AssetDirectory.EmptyTexturePass;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.extraUpdates = 1;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
            Projectile.timeLeft = 300;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.localAI[2]++;
        }

        public override void AI()
        {
            //ai0控制追踪
            if (Projectile.ai[0] > 0 && Projectile.localAI[2] < 1)
            {
                Projectile.Chase(600, 18,0.012f);
            }
            if (Projectile.localAI[2] > 0)
            {
                Projectile.velocity *= 0.9f;
                Projectile.Opacity -= 0.06f;
                if (Projectile.Opacity < 0.06f) Projectile.Kill();
            }

            if (Projectile.timeLeft % 1 == 0)
            {
                int dustcount = 2;
                for (int i = 0; i < dustcount; i++)
                {
                    //Vector2 offset = AyaUtils.RandAngle.ToRotationVector2() * Main.rand.NextFloat(2, 12);
                    //Vector2 pos = Projectile.Center + offset;
                    //Dust d = Dust.NewDustPerfect(pos, DustID.UltraBrightTorch, Projectile.velocity * 0.7f);
                    //d.noGravity = true;
                    Color color = HellSpirit.SpiritBlue;
                    if (Projectile.ai[0] > 0) color = HellSpirit.SpiritPurple;
                    Vector2 offset = (Projectile.velocity.ToRotation() + MathHelper.PiOver2).ToRotationVector2() * Main.rand.NextFloat(-8, 8) * 0.6f;
                    Vector2 vec = offset.ToRotation().ToRotationVector2() * offset.Length() * 0.125f;
                    if (Projectile.ai[0] > 0)
                    {
                        var ball = SoulsParticle.Spawn(Projectile.GetSource_FromAI(), Projectile.Center + offset, Projectile.velocity * 0f,
                            color.AdditiveColor() * Projectile.Opacity * 0.6f, 0.5f, 0.7f);
                        ball.SetAlphaFadeout(new Core.FloatModifier().SetAdditive(-0.05f));
                        ball.SetScaleFadeout(new Core.FloatModifier().SetAdditive(-0.06f));
                    }
                    else
                    {
                        var ball = SoulsParticle.Spawn(Projectile.GetSource_FromAI(), Projectile.Center + offset, Projectile.velocity * 0f,
                            color.AdditiveColor() * Projectile.Opacity * 0.6f, 0.4f, 0.7f);
                        ball.SetAlphaFadeout(new Core.FloatModifier().SetAdditive(-0.045f));
                        ball.SetScaleFadeout(new Core.FloatModifier().SetAdditive(-0.03f));
                    }
                    //ball.Scale = 0.6f;
                }
            }
        }
    }
    public class HellSpiritShotHoming : ModProjectile
    {
        public override string Texture => AssetDirectory.EmptyTexturePass;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.extraUpdates = 1;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
            Projectile.timeLeft = 300 * (1 + Projectile.extraUpdates);
        }
        public override bool? CanDamage() => Projectile.ai[1] > 120;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.localAI[2]++;
        }

        public override void AI()
        {
            Projectile.ai[1]++;
            //ai0控制追踪
            if (Projectile.ai[1] > 120 && Projectile.ai[0] > 0 && Projectile.localAI[2] < 1)
            {
                Projectile.Chase(800, 18, 0.012f);
            }
            else
            {
                Projectile.velocity *= 0.98f;
            }
            if (Projectile.localAI[2] > 0)
            {
                Projectile.velocity *= 0.9f;
                Projectile.Opacity -= 0.06f;
                if (Projectile.Opacity < 0.06f) Projectile.Kill();
            }

            if (Projectile.timeLeft % 1 == 0)
            {
                int dustcount = 2;
                for (int i = 0; i < dustcount; i++)
                {
                    //Vector2 offset = AyaUtils.RandAngle.ToRotationVector2() * Main.rand.NextFloat(2, 12);
                    //Vector2 pos = Projectile.Center + offset;
                    //Dust d = Dust.NewDustPerfect(pos, DustID.UltraBrightTorch, Projectile.velocity * 0.7f);
                    //d.noGravity = true;
                    Color color = HellSpirit.SpiritBlue;
                    if (Projectile.ai[0] > 0) color = HellSpirit.SpiritPurple;
                    Vector2 offset = (Projectile.velocity.ToRotation() + MathHelper.PiOver2).ToRotationVector2() * Main.rand.NextFloat(-8, 8) * 0.6f;
                    Vector2 vec = offset.ToRotation().ToRotationVector2() * offset.Length() * 0.125f;
                    if (Projectile.ai[0] > 0)
                    {
                        var ball = SoulsParticle.Spawn(Projectile.GetSource_FromAI(), Projectile.Center + offset, Projectile.velocity * 0f,
                            color.AdditiveColor() * Projectile.Opacity * 0.6f, 0.5f, 0.7f);
                        ball.SetAlphaFadeout(new Core.FloatModifier().SetAdditive(-0.05f));
                        ball.SetScaleFadeout(new Core.FloatModifier().SetAdditive(-0.06f));
                    }
                    else
                    {
                        var ball = SoulsParticle.Spawn(Projectile.GetSource_FromAI(), Projectile.Center + offset, Projectile.velocity * 0f,
                            color.AdditiveColor() * Projectile.Opacity * 0.6f, 0.4f, 0.7f);
                        ball.SetAlphaFadeout(new Core.FloatModifier().SetAdditive(-0.045f));
                        ball.SetScaleFadeout(new Core.FloatModifier().SetAdditive(-0.03f));
                    }
                    //ball.Scale = 0.6f;
                }
            }
        }
    }
}

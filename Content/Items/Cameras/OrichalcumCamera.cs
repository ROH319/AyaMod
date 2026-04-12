using AyaMod.Core.Prefabs;
using AyaMod.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using Terraria.GameContent;
using Terraria;
using Terraria.ID;
using Terraria.Enums;
using AyaMod.Core;
using ReLogic.Content;
using Terraria.DataStructures;

namespace AyaMod.Content.Items.Cameras
{
    public class OrichalcumCamera : BaseCamera
    {
        public override void SetOtherDefaults()
        {
            Item.width = 40;
            Item.height = 30;

            Item.damage = 72;

            Item.useTime = Item.useAnimation = 42;
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.shoot = ProjectileType<OrichalcumCameraProj>();
            Item.shootSpeed = 8;
            Item.knockBack = 6f;

            Item.SetShopValues(ItemRarityColor.Orange3, Item.sellPrice(0, 0, 40, 0));
            SetCameraStats(0.06f, 140, 1.6f, 0.5f);
            SetCaptureStats(1000, 60);
        }
    }

    public class OrichalcumCameraProj : BaseCameraProj
    {
        public override Color outerFrameColor => new Color(166, 143, 110);
        public override Color innerFrameColor => new Color(139, 121, 90) * 0.7f;
        public override Color focusCenterColor => base.focusCenterColor;
        public override Color flashColor => new Color(200, 180, 140).AdditiveColor() * 0.5f;
        public override void OnSnapInSight()
        {
            if (!Projectile.MyClient()) return;

            //int count = 6;
            //float baseSpeed = 6f;
            //float startRot = AyaUtils.RandAngle;
            //int damage = (int)(Projectile.damage * 0.35f);
            //for (int i = 0; i < count; i++)
            //{
            //    float factor = (float)i / count;
            //    Vector2 dir = (MathHelper.TwoPi * factor + startRot).ToRotationVector2();
            //    float speed = baseSpeed * Main.rand.NextFloat(0.8f, 1.2f);
            //    int type = ProjectileType<OrichalcumPetal>();
            //    var p = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center + dir * 12, dir * speed, type, damage, Projectile.knockBack / 4, Projectile.owner, ai0: Main.rand.NextFloat(MathHelper.TwoPi));
            //    p.rotation = p.velocity.ToRotation();
            //}
        }
        public override void PostAI()
        {
            if(!Projectile.MyClient()) return;
            if (Main.GameUpdateCount % 20 != 0 || !player.ItemAnimationActive) return;

            int count = 6;
            float baseSpeed = 13.5f;
            int damage = (int)(Projectile.damage * 0.15f);
            float startRot = Main.GameUpdateCount * 0.04f;
            int type = ProjectileType<OrichalcumPetal>();
            for (int i = 0; i < count; i++)
            {
                float factor = (float)i / count;
                Vector2 dir = (MathHelper.TwoPi * factor + startRot).ToRotationVector2();
                float xRot = dir.RotatedBy(MathHelper.PiOver2).ToRotation();
                float yRot = dir.RotatedBy(MathHelper.Pi).ToRotation();
                float xAcc = 0.018f;
                float yAcc = 0.3f;
                var p = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, dir * baseSpeed, type, damage, Projectile.knockBack / 4, Projectile.owner, xRot,yRot);
                p.localAI[0] = xAcc;
                p.localAI[1] = yAcc;
            }
        }
    }

    public class OrichalcumPetal : ModProjectile
    {
        // vanilla projectile texture id 221
        public override string Texture => AssetDirectory.VanillaProjPath(221);
        public ref float XRot => ref Projectile.ai[0];
        public ref float YRot => ref Projectile.ai[1];
        public ref float XAcc => ref Projectile.localAI[0];
        public ref float YAcc => ref Projectile.localAI[1];
        public override void SetStaticDefaults()
        {
            Projectile.SetTrail(4, 20);
            base.SetStaticDefaults();
        }

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 4;
            Projectile.extraUpdates = 0;
            Projectile.timeLeft = 120;
            Projectile.DamageType = ReporterDamage.Instance;
            Projectile.ArmorPenetration = 20;
            Projectile.scale = Main.rand.NextFloat(0.9f, 1.1f) * 1.25f;
            Projectile.SetImmune(20);
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.frame = Main.rand.Next(3);
        }
        public override void AI()
        {
            if (Projectile.timeLeft < 30) Projectile.Opacity -= 1f / 30f;
            
            Projectile.velocity += XRot.ToRotationVector2() * XAcc;
            Projectile.velocity += YRot.ToRotationVector2() * YAcc;
            Projectile.rotation = Projectile.velocity.ToRotation();

            if(Main.GameUpdateCount%3==0)
            {
                //Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.PinkTorch,Alpha:128);
                //d.scale *= 2.5f;
                //d.noGravity = true;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;

            Rectangle frame = new(0, 0 + 20 * Projectile.frame, 20, 20);
            Vector2 origin = frame.Size() / 2f;

            int len = Projectile.oldPos.Length;
            for (int i = 0; i < len; i++)
            {
                if (Projectile.oldPos[i] == Vector2.Zero || i % 2 != 0) continue;
                float factor = 1f - (float)i / len;
                Vector2 drawPos = Projectile.oldPos[i] + Projectile.Size / 2f - Main.screenPosition;
                float rot = Projectile.oldRot[i];
                Color color = Color.White * factor * Projectile.Opacity * 0.25f;
                //color *= Lighting.GetColor((int)(drawPos.X / 16f), (int)(drawPos.Y / 16f)).ToVector3().Length();
                Main.spriteBatch.Draw(texture, drawPos, frame, color, rot, origin, Projectile.scale, SpriteEffects.None, 0f);
            }

            RenderHelper.DrawBloom(12, 4, texture, Projectile.Center - Main.screenPosition, frame, Color.LightPink.AdditiveColor() * Projectile.Opacity * 0.2f, Projectile.rotation, origin, Projectile.scale,0);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, frame, Color.White * Projectile.Opacity, Projectile.rotation, origin, Projectile.scale, 0, 0);

            return false;
        }
    }
}

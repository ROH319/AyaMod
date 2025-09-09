using AyaMod.Core.Prefabs;
using AyaMod.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent;
using Terraria;
using Terraria.ID;
using Terraria.Enums;
using AyaMod.Core;

namespace AyaMod.Content.Items.Cameras
{
    public class JungleCamera2 : BaseCamera
    {
        public override void SetOtherDefaults()
        {
            Item.width = 52;
            Item.height = 48;

            Item.damage = 80;

            Item.useTime = Item.useAnimation = 50;
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.shoot = ProjectileType<JungleCameraProj2>();
            Item.shootSpeed = 8;
            Item.knockBack = 9f;

            Item.SetShopValues(ItemRarityColor.Lime7, Item.sellPrice(0, 0, 60, 0));
            SetCameraStats(0.08f, 164, 1.5f,0.6f);
            SetCaptureStats(100, 5);
        }
    }

    public class JungleCameraProj2 : BaseCameraProj
    {
        public override Color outerFrameColor => new Color(165, 228, 138);
        public override Color innerFrameColor => new Color(182, 196, 28) * 0.7f;
        public override Color focusCenterColor => base.focusCenterColor;
        public override Color flashColor => new Color(107, 203, 0).AdditiveColor() * 0.5f;

        public override void OnSnapInSight()
        {
            if (!Projectile.MyClient()) return;

            int spread = 16;
            float startRot = AyaUtils.RandAngle;
            for (int i = 0; i < spread; i++)
            {
                float factor = (float)i / spread;
                Vector2 dir = (MathHelper.TwoPi / spread * i + startRot).ToRotationVector2();
                float rotdir = 1;
                if (i % 2 == 0) rotdir *= -1;
                int type = ModContent.ProjectileType<JungleCameraLeaf2>();
                Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center + dir * 40, dir, type, (int)(Projectile.damage * 0.75f), Projectile.knockBack / 4, Projectile.owner, 0.1f * rotdir);
            }
        }
    }

    public class JungleCameraLeaf2 : ModProjectile
    {
        public override string Texture => AssetDirectory.VanillaProjPath(976);
        public override void SetStaticDefaults()
        {
            Projectile.SetTrail(2, 60);
        }
        public override void SetDefaults()
        {

            Projectile.width = Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 8;
            Projectile.scale = 1f + (float)Main.rand.Next(30) * 0.01f;
            Projectile.extraUpdates = 0;
            Projectile.timeLeft = 90 * Projectile.MaxUpdates;

            Projectile.ArmorPenetration = 20;
        }

        public override void AI()
        {
            float num = (float)Math.PI / 2f;


            if (Collision.LavaCollision(Projectile.position, Projectile.width, Projectile.height))
            {
                Projectile.Kill();
                return;
            }

            if (++Projectile.frameCounter >= 2)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 7)
                    Projectile.frame = 0;
            }
            float timeleftFactor = Projectile.TimeleftFactor();
            float fromValue = 60 - Projectile.timeLeft;
            float fromMin = Projectile.ai[1];
            float fromMax = Projectile.ai[1] + 20f;
            float num4 = Utils.Remap(fromValue, fromMin, fromMax, 0f, 1f) 
                * Utils.Remap(fromValue, fromMin, Projectile.ai[1] + 60f, 1f, 0f);

            float rotspeedFactor = Utils.Remap(timeleftFactor, 0.4f, 1f, 0.75f, 1f)
                /*+ Utils.Remap(timeleftFactor, 0f, 0.6f, 0.3f, 0.125f) + 0.3f*/;
            Projectile.velocity = Projectile.velocity.RotatedBy(Projectile.ai[0] * rotspeedFactor);
            Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.UnitY) * (16f - 12f * num4);
            Projectile.Opacity = Utils.Remap(fromValue, 0f, 10f, 0f, 1f) * Utils.Remap(fromValue, 30f, 60f, 1f, 0f);
            num = 0f;
            if (Main.rand.Next(2) == 0)
            {
                Dust dust2 = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, 40, 0f, 0f, 0, default(Color), 1.2f);
                dust2.noGravity = true;
                dust2.velocity = Projectile.velocity * 0.5f;
            }

            if (Projectile.localAI[1] == 0f)
            {
                Projectile.localAI[1] = 1f;
                for (int i = 0; i < 10; i++)
                {
                    Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, 40, 0f, 0f, 0, default(Color), 0.7f).velocity = Projectile.velocity.SafeNormalize(Vector2.UnitY).RotatedBy(Main.rand.NextFloat() * ((float)Math.PI * 2f) * 0.25f) * (Main.rand.NextFloat() * 3f);
                }
            }

            Projectile.rotation = Projectile.velocity.ToRotation();

        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;

            // 计算光照颜色
            Vector2 positionCenter = new Vector2(
                Projectile.position.X + Projectile.width * 0.5f,
                Projectile.position.Y + Projectile.height * 0.5f);
            Color baseColor = Lighting.GetColor((int)(positionCenter.X / 16), (int)(positionCenter.Y / 16f));

            // 绘制配置
            const float MAX_SCALE = 1.3f;
            const int SEGMENTS = 18;
            const int TRAIL_STEP = -3;
            const float SEGMENT_INTERPOLATION = 15f;
            const int ANIMATION_CYCLE_HEIGHT = 20 * 7; // 7 frames height

            Rectangle baseFrame = new Rectangle(0, 20 * Projectile.frame, 32, 20);
            Vector2 spriteOrigin = baseFrame.Size() / 2f;
            SpriteEffects drawDirection = SpriteEffects.None;

            float lifetimeFactor = Projectile.TimeleftFactor();
            float lifetimefactor2 = Utils.Remap(lifetimeFactor, 0, 1f, 0.5f, 1f);
            // 绘制拖尾
            for (int segmentIndex = SEGMENTS; segmentIndex >= 0; segmentIndex += TRAIL_STEP)
            {

                float baseRotation = Projectile.oldRot[segmentIndex];
                Vector2 segmentPosition = Projectile.oldPos[segmentIndex];

                // 处理帧动画
                baseFrame.Y += baseFrame.Height;
                baseFrame.Y %= ANIMATION_CYCLE_HEIGHT;

                // 计算每个拖尾段的颜色衰减
                Vector2 lightPosition = segmentPosition + Projectile.Size / 2f;
                Color segmentColor = baseColor * (Lighting.GetColor(lightPosition.ToTileCoordinates()).ToVector3().Length() / 1.74f);
                float alphafactor = 18 - segmentIndex;
                segmentColor *= alphafactor / ((float)ProjectileID.Sets.TrailCacheLength[Projectile.type] * 1f);
                segmentColor *= lifetimefactor2;
                // 计算拖尾位置
                Vector2 drawPosition = segmentPosition + Projectile.Size / 2f - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);

                // 计算拖尾缩放
                float scaleInterpolation = MathHelper.Lerp(Projectile.scale, MAX_SCALE, segmentIndex / SEGMENT_INTERPOLATION);

                // 绘制拖尾
                Main.EntitySpriteDraw(
                    texture,
                    drawPosition,
                    baseFrame,
                    segmentColor,
                    baseRotation,
                    spriteOrigin,
                    scaleInterpolation,
                    drawDirection,
                    0);
            }

            Color bloomColor = baseColor.AdditiveColor() * lifetimeFactor * 0.2f;
            RenderHelper.DrawBloom(6, 4, texture, Projectile.Center - Main.screenPosition, baseFrame, bloomColor, Projectile.rotation, spriteOrigin, Projectile.scale);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, baseFrame, baseColor * lifetimeFactor, Projectile.rotation, spriteOrigin, Projectile.scale, drawDirection, 0);


            return false;
        }
    }
}
